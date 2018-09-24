namespace IdentityServer4.NHibernate.Stores
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using IdentityServer4.Stores;
    using IdentityServer4.Models;
    using Microsoft.Extensions.Logging;
    using global::NHibernate;

    /// <summary>
    /// Implementation of the NHibernate-based IResourceStore.
    /// </summary>
    public class ResourceStore : IResourceStore
    {
        private readonly ISession _session;
        private readonly ILogger<ResourceStore> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceStore"/> class.
        /// </summary>
        /// <param name="session">The NHibernate session used to retrieve the data.</param>
        /// <param name="logger">The logger.</param>
        /// <exception cref="ArgumentNullException">session</exception>
        public ResourceStore(ISession session, ILogger<ResourceStore> logger)
        {
            _session = session ?? throw new ArgumentNullException(nameof(session));
            _logger = logger;
        }

        /// <summary>
        /// Finds the API resource by name.
        /// </summary>
        public Task<Models.ApiResource> FindApiResourceAsync(string name)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets API resources by scope names.
        /// </summary>
        /// <param name="scopeNames">Scope name/names.</param>
        public Task<IEnumerable<ApiResource>> FindApiResourcesByScopeAsync(IEnumerable<string> scopeNames)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets identity resources by scope name.
        /// </summary>
        /// <param name="scopeNames">Scope name/names.</param>
        public Task<IEnumerable<Models.IdentityResource>> FindIdentityResourcesByScopeAsync(IEnumerable<string> scopeNames)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets all resources.
        /// </summary>
        public Task<Resources> GetAllResourcesAsync()
        {
            throw new NotImplementedException();
        }
    }
}