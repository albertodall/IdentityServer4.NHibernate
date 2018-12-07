﻿using System;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.NHibernate.Entities;
using IdentityServer4.Stores;
using IdentityServer4.Stores.Serialization;
using Microsoft.Extensions.Logging;
using NHibernate;

namespace IdentityServer4.NHibernate.Stores
{
    /// <summary>
    /// Implementation of the NHibernate-based device flow store.
    /// </summary>
    public class DeviceFlowStore : IDeviceFlowStore
    {
        private readonly ISession _session;
        private readonly ILogger _logger;
        private readonly IPersistentGrantSerializer _serializer;

        public DeviceFlowStore(ISession session, IPersistentGrantSerializer serializer, ILogger<DeviceFlowStore> logger)
        {
            _session = session ?? throw new ArgumentNullException(nameof(session));
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<DeviceCode> FindByDeviceCodeAsync(string deviceCode)
        {
            DeviceCode model = null;
            using (var tx = _session.BeginTransaction())
            {
                var query = _session.QueryOver<DeviceFlowCodes>()
                    .Where(c => c.DeviceCode == deviceCode);

                var result = await query.SingleOrDefaultAsync();
                model = ToModel(result?.Data);
            }

            _logger.LogDebug("{deviceCode} found in database: {deviceCodeFound}", deviceCode, model != null);

            return model;
        }

        public async Task<DeviceCode> FindByUserCodeAsync(string userCode)
        {
            DeviceCode model = null;
            using (var tx = _session.BeginTransaction())
            {
                var result = await _session.GetAsync<DeviceFlowCodes>(userCode);
                model = ToModel(result?.Data);
            }

            _logger.LogDebug("{userCode} found in database: {userCodeFound}", userCode, model != null);

            return model;
        }

        public async Task RemoveByDeviceCodeAsync(string deviceCode)
        {
            using (var tx = _session.BeginTransaction())
            {
                var deviceFlowCodes = await _session.QueryOver<DeviceFlowCodes>()
                    .Where(c => c.DeviceCode == deviceCode)
                    .SingleOrDefaultAsync();

                if (deviceFlowCodes != null)
                {
                    _logger.LogDebug("removing {deviceCode} device code from database", deviceCode);

                    try
                    {
                        await _session.DeleteAsync(deviceFlowCodes);
                        await tx.CommitAsync();
                    }
                    catch (HibernateException ex)
                    {
                        _logger.LogInformation("exception removing {deviceCode} device code from database: {error}", deviceCode, ex.Message);
                    }
                }
                else
                {
                    _logger.LogDebug("no {deviceCode} device code found in database", deviceCode);
                }
            }
        }

        public async Task StoreDeviceAuthorizationAsync(string deviceCode, string userCode, DeviceCode data)
        {
            using (var tx = _session.BeginTransaction())
            {
                await _session.SaveAsync(ToEntity(data, deviceCode, userCode));
                await tx.CommitAsync();
            }
        }

        public async Task UpdateByUserCodeAsync(string userCode, DeviceCode data)
        {
            using (var tx = _session.BeginTransaction())
            {
                var codeToUpdate = _session.Get<DeviceFlowCodes>(userCode);
                if (codeToUpdate == null)
                {
                    _logger.LogError("{userCode} not found in database", userCode);
                    throw new InvalidOperationException("Could not update device code");
                }

                var entity = ToEntity(data, codeToUpdate.DeviceCode, userCode);
                _logger.LogDebug("{userCode} found in database", userCode);

                codeToUpdate.SubjectId = data.Subject?.FindFirst(JwtClaimTypes.Subject).Value;
                codeToUpdate.Data = entity.Data;

                try
                {
                    await _session.SaveAsync(codeToUpdate);
                    await tx.CommitAsync();
                }
                catch (HibernateException ex)
                {
                    _logger.LogWarning("exception updating {userCode} user code in database: {error}", userCode, ex.Message);
                }
            }
        }

        private DeviceFlowCodes ToEntity(DeviceCode model, string deviceCode, string userCode)
        {
            if (model == null || deviceCode == null || userCode == null) return null;

            return new DeviceFlowCodes
            {
                DeviceCode = deviceCode,
                ID = userCode,
                ClientId = model.ClientId,
                SubjectId = model.Subject?.FindFirst(JwtClaimTypes.Subject).Value,
                CreationTime = model.CreationTime,
                Expiration = model.CreationTime.AddSeconds(model.Lifetime),
                Data = _serializer.Serialize(model)
            };
        }

        private DeviceCode ToModel(string entity)
        {
            if (entity == null) return null;

            return _serializer.Deserialize<DeviceCode>(entity);
        }
    }
}
