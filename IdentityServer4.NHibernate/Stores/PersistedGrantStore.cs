using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.NHibernate.Extensions;
using IdentityServer4.Stores;
using NHibernate;
using Microsoft.Extensions.Logging;

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
        /// Gets a single grant by key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>The grant.</returns>
        public async Task<PersistedGrant> GetAsync(string key)
        {
            var persistedGrant = await _session.QueryOver<Entities.PersistedGrant>()
                .Where(g => g.ID == key)
                .SingleOrDefaultAsync();
            var model = persistedGrant?.ToModel();

            _logger.LogDebug("{persistedGrantKey} found in database: {persistedGrantKeyFound}", key, model != null);

            return await Task.FromResult(model);
        }

        public async Task RemoveAllAsync(string subjectId, string clientId)
        {
            string deleteHql = "delete from PersistedGrant where SubjectId = :subjectId and ClientId = :clientId";
            int persistedGrantsCount = 0;

            using (var tx = _session.BeginTransaction())
            {
                try
                {
                    persistedGrantsCount = await _session.QueryOver<Entities.PersistedGrant>()
                        .Where(g => g.SubjectId == subjectId)
                        .And(g => g.ClientId == clientId)
                        .RowCountAsync();
                
                    await _session.CreateQuery(deleteHql)
                        .SetString("subjectId", subjectId)
                        .SetString("clientId", clientId)
                        .ExecuteUpdateAsync();

                    _logger.LogDebug("removing {persistedGrantCount} persisted grants from database for subject {subjectId}, clientId {clientId}", 
                        persistedGrantsCount, subjectId, clientId);

                    await tx.CommitAsync();
                }
                catch (HibernateException ex)
                {
                    _logger.LogInformation("Removing {persistedGrantCount} persisted grants from database for subject {subjectId}, clientId {clientId}: {error}", 
                        persistedGrantsCount, subjectId, clientId, ex.Message);
                }
            }
                
        }

        /// <summary>
        /// Removes all grants of a given type for a given subject id and client id.
        /// </summary>
        /// <param name="subjectId">The subject identifier.</param>
        /// <param name="clientId">The client identifier.</param>
        /// <param name="type">The type.</param>
        public async Task RemoveAllAsync(string subjectId, string clientId, string type)
        {
            string deleteHql = "delete from PersistedGrant where SubjectId = :subjectId and ClientId = :clientId and Type = :type";
            int persistedGrantsCount = 0;

            using (var tx = _session.BeginTransaction())
            {
                try
                {
                    persistedGrantsCount = await _session.QueryOver<Entities.PersistedGrant>()
                        .Where(g => g.SubjectId == subjectId)
                        .And(g => g.ClientId == clientId)
                        .And(g => g.Type == type)
                        .RowCountAsync();

                    await _session.CreateQuery(deleteHql)
                        .SetString("subjectId", subjectId)
                        .SetString("clientId", clientId)
                        .SetString("type", type)
                        .ExecuteUpdateAsync();

                    _logger.LogDebug("removing {persistedGrantCount} persisted grants from database for subject {subjectId}, clientId {clientId}, grantType {persistedGrantType}", 
                        persistedGrantsCount, subjectId, clientId, type);

                    await tx.CommitAsync();
                }
                catch (HibernateException ex)
                {
                    _logger.LogInformation("exception removing {persistedGrantCount} persisted grants from database for subject {subjectId}, clientId {clientId}, grantType {persistedGrantType}: {error}", 
                        persistedGrantsCount, subjectId, clientId, type, ex.Message);
                }
            }
        }

        /// <summary>
        /// Removes the grant by key.
        /// </summary>
        /// <param name="key">The key.</param>
        public async Task RemoveAsync(string key)
        {
            using (var tx = _session.BeginTransaction())
            {
                var persistentGrantToDelete = _session.Get<Entities.PersistedGrant>(key);
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
    }
}
