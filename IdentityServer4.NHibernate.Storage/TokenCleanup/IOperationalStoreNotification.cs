using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityServer4.NHibernate.Entities;

namespace IdentityServer4.NHibernate.TokenCleanup
{
    /// <summary>
    /// Interface to model notifications from the TokenCleanup feature.
    /// </summary>
    public interface IOperationalStoreNotification
    {
        /// <summary>
        /// Notification for persisted grants being removed.
        /// </summary>
        /// <param name="persistedGrants"></param>
        Task PersistedGrantsRemovedAsync(IEnumerable<PersistedGrant> persistedGrants);
    }
}
