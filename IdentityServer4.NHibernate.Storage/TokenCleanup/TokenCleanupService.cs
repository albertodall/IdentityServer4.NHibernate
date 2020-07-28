﻿using System;
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
            const string deleteExpiredGrantsHql = "delete PersistedGrant pg where pg.ID in (:expiredGrantsIDs)";

            var expiredTokenFound = int.MaxValue;

            while (expiredTokenFound >= _options.TokenCleanupBatchSize)
            {
                using (var tx = _session.BeginTransaction())
                {
                    var expiredGrantsQuery = _session.QueryOver<PersistedGrant>()
                        .Where(g => g.Expiration < DateTime.UtcNow)
                        .OrderBy(g => g.ID).Asc
                        .Take(_options.TokenCleanupBatchSize);

                    var expiredGrants = await expiredGrantsQuery.ListAsync();
                    var expiredGrantsIDs = expiredGrants.Select(pg => pg.ID).ToArray();
                    expiredTokenFound = expiredGrantsIDs.Length;

                    if (expiredTokenFound > 0)
                    {
                        _logger.LogInformation($"Removing {expiredTokenFound} expired grants");

                        await _session.CreateQuery(deleteExpiredGrantsHql)
                            .SetParameterList("expiredGrantsIDs", expiredGrantsIDs)
                            .ExecuteUpdateAsync();

                        await tx.CommitAsync();

                        if (_operationalStoreNotification != null)
                        {
                            await _operationalStoreNotification.PersistedGrantsRemovedAsync(expiredGrants);
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
            const string deleteExpiredCodesHql = "delete DeviceFlowCodes c where c.ID in (:expiredCodesIDs)";

            var expiredDeviceCodesFound = int.MaxValue;

            while (expiredDeviceCodesFound >= _options.TokenCleanupBatchSize)
            {
                using (var tx = _session.BeginTransaction())
                {
                    var expiredCodesQuery = _session.QueryOver<DeviceFlowCodes>()
                        .Where(c => c.Expiration < DateTime.UtcNow)
                        .OrderBy(c => c.ID).Asc
                        .Take(_options.TokenCleanupBatchSize);

                    var expiredCodes = await expiredCodesQuery.ListAsync();
                    var expiredCodesIDs = expiredCodes.Select(c => c.ID).ToArray();
                    expiredDeviceCodesFound = expiredCodesIDs.Length;

                    if (expiredDeviceCodesFound > 0)
                    {
                        _logger.LogInformation("Removing {deviceCodeCount} device flow codes", expiredDeviceCodesFound);

                        await _session.CreateQuery(deleteExpiredCodesHql)
                            .SetParameterList("expiredCodesIDs", expiredCodesIDs)
                            .ExecuteUpdateAsync();

                        await tx.CommitAsync();

                        if (_operationalStoreNotification != null)
                        {
                            await _operationalStoreNotification.DeviceCodesRemovedAsync(expiredCodes);
                        }
                    }
                }
            }
        }
    }
}
