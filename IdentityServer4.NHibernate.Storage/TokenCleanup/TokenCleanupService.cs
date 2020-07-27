using System;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.NHibernate.Entities;
using IdentityServer4.NHibernate.Options;
using Microsoft.Extensions.Logging;
using NHibernate;

namespace IdentityServer4.NHibernate
{
    /// <summary>
    /// Periodically cleans up expired persisted grants.
    /// </summary>
    public class TokenCleanupService
    {
        private readonly OperationalStoreOptions _options;
        private readonly IStatelessSession _session;
        private readonly IOperationalStoreNotification _operationalStoreNotification;
        private readonly ILogger<TokenCleanupService> _logger;

        /// <summary>
        /// Constructor for TokenCleanupService.
        /// </summary>
        public TokenCleanupService(
            OperationalStoreOptions options,
            IStatelessSession session,
            ILogger<TokenCleanupService> logger,
            IOperationalStoreNotification operationalStoreNotification = null)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
            if (_options.TokenCleanupBatchSize < 1) throw new ArgumentException("Token cleanup batch size interval must be at least 1");

            _session = session ?? throw new ArgumentNullException(nameof(session));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _operationalStoreNotification = operationalStoreNotification;
        }

        /// <summary>
        /// Method to clear expired persisted grants.
        /// </summary>
        public async Task RemoveExpiredGrantsAsync()
        {
            try
            {
                _logger.LogTrace("Querying for expired grants to remove");

                await RemoveGrantsAsync();
                await RemoveDeviceCodesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception removing expired grants: {exception}", ex.Message);
            }
        }

        /// <summary>
        /// Removes the stale persisted grants.
        /// </summary>
        protected virtual async Task RemoveGrantsAsync()
        {
            const string deleteExpiredGrantsHql = "delete PersistedGrants pg where pg.ID in (:expiredGrantsIDs)";

            var found = int.MaxValue;

            while (found >= _options.TokenCleanupBatchSize)
            {
                using (var tx = _session.BeginTransaction())
                {
                    var expiredGrantsQuery = _session.QueryOver<PersistedGrant>()
                        .Where(g => g.CreationTime < DateTimeOffset.UtcNow)
                        .OrderBy(g => g.ID).Asc
                        .Select(g => g.ID)
                        .Take(_options.TokenCleanupBatchSize);

                    var expiredGrantsIDs = (await expiredGrantsQuery.ListAsync()).ToArray();
                    found = expiredGrantsIDs.Length;

                    if (found > 0)
                    {
                        _logger.LogInformation($"Removing {found} expired grants");

                        await _session.CreateQuery(deleteExpiredGrantsHql)
                            .SetParameterList("expiredGrantsIDs", expiredGrantsIDs)
                            .ExecuteUpdateAsync();

                        await tx.CommitAsync();

                        if (_operationalStoreNotification != null)
                        {
                            await _operationalStoreNotification.PersistedGrantsRemovedAsync(expiredGrantsIDs);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Removes the stale device codes.
        /// </summary>
        protected virtual async Task RemoveDeviceCodesAsync()
        {
            const string deleteExpiredCodesHql = "delete DeviceCodes c where c.ID in (:expiredCodesIDs)";

            var found = int.MaxValue;

            while (found >= _options.TokenCleanupBatchSize)
            {
                using (var tx = _session.BeginTransaction())
                {
                    var expiredCodesQuery = _session.QueryOver<DeviceFlowCodes>()
                        .Where(c => c.CreationTime < DateTimeOffset.UtcNow)
                        .OrderBy(c => c.ID).Asc
                        .Select(c => c.ID)
                        .Take(_options.TokenCleanupBatchSize);

                    var expiredCodesIDs = (await expiredCodesQuery.ListAsync()).ToArray();
                    found = expiredCodesIDs.Length;

                    if (found > 0)
                    {
                        _logger.LogInformation("Removing {deviceCodeCount} device flow codes", found);

                        await _session.CreateQuery(deleteExpiredCodesHql)
                            .SetParameterList("expiredGrantsIDs", expiredCodesIDs)
                            .ExecuteUpdateAsync();

                        await tx.CommitAsync();

                        if (_operationalStoreNotification != null)
                        {
                            await _operationalStoreNotification.DeviceCodesRemovedAsync(expiredCodesIDs);
                        }
                    }
                }
            }
        }
    }
}
