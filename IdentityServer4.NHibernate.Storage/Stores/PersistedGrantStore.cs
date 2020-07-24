using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.NHibernate.Extensions;
using IdentityServer4.Stores;
using Microsoft.Extensions.Logging;
using NHibernate;

namespace IdentityServer4.NHibernate.Stores
{
    /// <summary>
    /// Implementation of the NHibernate-based persisted grant store.
    /// </summary>
    public class PersistedGrantStore : IPersistedGrantStore
    {
        private readonly ISession _session;
        private readonly ILogger<PersistedGrantStore> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="PersistedGrantStore"/> class.
        /// </summary>
        /// <param name="session">The NHibernate session used to retrieve the data.</param>
        /// <param name="logger">The logger.</param>
        public PersistedGrantStore(ISession session, ILogger<PersistedGrantStore> logger)
        {
            _session = session ?? throw new ArgumentNullException(nameof(session));
            _logger = logger;
        }

        /// <summary>
        /// Stores the grant.
        /// </summary>
        /// <param name="grant">The grant to store.</param>
        public async Task StoreAsync(PersistedGrant grant)
        {
            using (var tx = _session.BeginTransaction())
            {
                try
                {
                    var existingGrant = await _session.GetAsync<Entities.PersistedGrant>(grant.Key);
                    if (existingGrant == null)
                    {
                        _logger.LogDebug("{persistedGrantKey} not found in database. Creating it.", grant.Key);

                        await _session.SaveAsync(grant.ToEntity());
                    }
                    else
                    {
                        _logger.LogDebug("{persistedGrantKey} found in database. Updating it", grant.Key);

                        grant.UpdateEntity(existingGrant);
                        await _session.UpdateAsync(existingGrant);
                    }
                    await tx.CommitAsync();
                }
                catch (HibernateException ex)
                {
                    _logger.LogWarning("exception storing {persistedGrantKey} persisted grant in database: {error}",
                        grant.Key, ex.Message);
                }
            }
        }

        /// <summary>
        /// Gets a single grant by key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>The grant.</returns>
        public async Task<PersistedGrant> GetAsync(string key)
        {
            var persistedGrant = await _session.GetAsync<Entities.PersistedGrant>(key);

            var model = persistedGrant?.ToModel();

            _logger.LogDebug("{persistedGrantKey} found in database: {persistedGrantKeyFound}", key, model != null);

            return model;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<PersistedGrant>> GetAllAsync(PersistedGrantFilter filter)
        {
            filter.Validate();

            var persistedGrantsQuery = ApplyPersistentGrantFilter(_session.QueryOver<Entities.PersistedGrant>(), filter);

            var results = await persistedGrantsQuery.ListAsync();

            var model = results.Select(x => x.ToModel());

            _logger.LogDebug("{persistedGrantCount} persisted grants found for {@filter}", results.Count, filter);

            return model;
        }

        /// <summary>
        /// Gets all grants for a given subject id.
        /// </summary>
        /// <param name="subjectId">The subject identifier.</param>
        /// <returns>The list of grants.</returns>
        public async Task<IEnumerable<PersistedGrant>> GetAllAsync(string subjectId)
        {
            var persistedGrantsQuery = _session.QueryOver<Entities.PersistedGrant>()
                .Where(g => g.SubjectId == subjectId);

            var persistedGrants = await persistedGrantsQuery.ListAsync();

            _logger.LogDebug("{persistedGrantCount} persisted grants found for {subjectId}", persistedGrants.Count, subjectId);

            return persistedGrants.Select(g => g.ToModel()).ToArray();
        }

        /// <summary>
        /// Removes the grant by key.
        /// </summary>
        /// <param name="key">The key.</param>
        public async Task RemoveAsync(string key)
        {
            using (var tx = _session.BeginTransaction())
            {
                var persistentGrantToDelete = await _session.GetAsync<Entities.PersistedGrant>(key);
                if (persistentGrantToDelete != null)
                {
                    _logger.LogDebug("removing {persistedGrantKey} persisted grant from database", key);

                    try
                    {
                        await _session.DeleteAsync(persistentGrantToDelete);
                        await tx.CommitAsync();
                    }
                    catch (HibernateException ex)
                    {
                        _logger.LogInformation("exception removing {persistedGrantKey} persisted grant from database: {error}", key, ex.Message);
                    }
                }
                else
                {
                    _logger.LogDebug("no {persistedGrantKey} persisted grant found in database", key);
                }
            }
        }

        /// <summary>
        /// Removes all grants for a given filter condition.
        /// </summary>
        public async Task RemoveAllAsync(PersistedGrantFilter filter)
        {
            filter.Validate();

            const string deleteHql = "delete from PersistedGrant where 1=1";
            int persistedGrantsCount = 0;

            using (var tx = _session.BeginTransaction())
            {
                try
                {
                    persistedGrantsCount =
                        await ApplyPersistentGrantFilter(_session.QueryOver<Entities.PersistedGrant>(), filter)
                            .RowCountAsync();

                    var filteredDeleteHql = CreateFilteredDeleteHqlStatement(deleteHql, filter);
                    var query = ApplyPersistedGrantFilter(_session, filteredDeleteHql, filter);
                    await query.ExecuteUpdateAsync();

                    await tx.CommitAsync();

                    _logger.LogDebug("removing {persistedGrantCount} persisted grants from database for {@filter}",
                        persistedGrantsCount, filter);
                }
                catch (HibernateException ex)
                {
                    _logger.LogInformation("Removing {persistedGrantCount} persisted grants from database for {@filter}: {error}",
                        persistedGrantsCount, filter, ex.Message);
                }
            }
        }

        private static IQueryOver<Entities.PersistedGrant, Entities.PersistedGrant> ApplyPersistentGrantFilter(
            IQueryOver<Entities.PersistedGrant, Entities.PersistedGrant> query,
            PersistedGrantFilter filter)
        {
            if (!string.IsNullOrWhiteSpace(filter.SubjectId))
            {
                query.Where(grant => grant.SubjectId == filter.SubjectId);
            }

            if (!string.IsNullOrWhiteSpace(filter.SessionId))
            {
                query.Where(grant => grant.SubjectId == filter.SubjectId);
            }

            if (!string.IsNullOrWhiteSpace(filter.ClientId))
            {
                query.Where(grant => grant.ClientId == filter.SubjectId);
            }

            if (!string.IsNullOrWhiteSpace(filter.Type))
            {
                query.Where(grant => grant.Type == filter.Type);
            }

            return query;
        }

        private static string CreateFilteredDeleteHqlStatement(string hqlStatement, PersistedGrantFilter filter)
        {
            var hqlStringBuilder = new StringBuilder(hqlStatement);

            if (!string.IsNullOrWhiteSpace(filter.SubjectId))
            {
                hqlStringBuilder.Append(" and SubjectId = :subjectId");
            }

            if (!string.IsNullOrWhiteSpace(filter.SessionId))
            {
                hqlStringBuilder.Append(" and SessionId = :sessionId");
            }

            if (!string.IsNullOrWhiteSpace(filter.ClientId))
            {
                hqlStringBuilder.Append(" and ClientId = :clientId");
            }

            if (!string.IsNullOrWhiteSpace(filter.Type))
            {
                hqlStringBuilder.Append(" and Type = :type");
            }

            return hqlStringBuilder.ToString();
        }

        private static IQuery ApplyPersistedGrantFilter(ISession session, string hqlStatement, PersistedGrantFilter filter)
        {
            var hqlQuery = session.CreateQuery(hqlStatement);

            if (!string.IsNullOrWhiteSpace(filter.SubjectId))
            {
                hqlQuery.SetString("subjectId", filter.SubjectId);
            }

            if (!string.IsNullOrWhiteSpace(filter.SessionId))
            {
                hqlQuery.SetString("sessionId", filter.SessionId);
            }

            if (!string.IsNullOrWhiteSpace(filter.ClientId))
            {
                hqlQuery.SetString("clientId", filter.ClientId);
            }

            if (!string.IsNullOrWhiteSpace(filter.Type))
            {
                hqlQuery.SetString("type", filter.Type);
            }

            return hqlQuery;
        }
    }
}
