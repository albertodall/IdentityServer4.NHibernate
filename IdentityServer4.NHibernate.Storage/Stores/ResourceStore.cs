using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.NHibernate.Entities;
using IdentityServer4.NHibernate.Extensions;
using IdentityServer4.Stores;
using Microsoft.Extensions.Logging;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;

namespace IdentityServer4.NHibernate.Stores
{
    /// <summary>
    /// Implementation of the NHibernate-based IResourceStore.
    /// </summary>
    /// <seealso cref="IdentityServer4.Stores.IResourceStore" />
    public class ResourceStore : IResourceStore
    {
        private readonly ISession _session;
        private readonly ILogger<ResourceStore> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceStore"/> class.
        /// </summary>
        /// <param name="session">The NHibernate session used to retrieve the data.</param>
        /// <param name="logger">The logger.</param>
        public ResourceStore(ISession session, ILogger<ResourceStore> logger)
        {
            _session = session ?? throw new ArgumentNullException(nameof(session));
            _logger = logger;
        }

        /// <summary>
        /// Finds the API resources by name.
        /// </summary>
        /// <param name="apiResourceNames">The names.</param>
        /// <returns></returns>
        public async Task<IEnumerable<Models.ApiResource>> FindApiResourcesByNameAsync(IEnumerable<string> apiResourceNames)
        {
            var resourcesQuery = _session.QueryOver<Entities.ApiResource>()
                .Fetch(SelectMode.Fetch, api => api.Secrets)
                .Fetch(SelectMode.Fetch, api => api.Scopes)
                .Fetch(SelectMode.Fetch, api => api.UserClaims)
                .Fetch(SelectMode.Fetch, api => api.Properties)
                .Where(r => r.Name.IsIn(apiResourceNames.ToList()))
                .TransformUsing(Transformers.DistinctRootEntity);

            var result = await resourcesQuery.ListAsync();

            var models = result.Select(x => x.ToModel()).ToArray();

            if (result.Any())
            {
                _logger.LogDebug("Found {apis} API resource in database", result.Select(x => x.Name));
            }
            else
            {
                _logger.LogDebug("Did not find {apis} API resource in database", apiResourceNames);
            }

            return models;
        }

        /// <summary>
        /// Gets API resources by scope name.
        /// </summary>
        /// <param name="scopeNames">Scope name/names.</param>
        /// <returns></returns>
        public async Task<IEnumerable<Models.ApiResource>> FindApiResourcesByScopeNameAsync(IEnumerable<string> scopeNames)
        {
            var resourcesQuery = _session.QueryOver<Entities.ApiResource>()
                .Fetch(SelectMode.Fetch, api => api.Secrets)
                .Fetch(SelectMode.Fetch, api => api.Scopes)
                .Fetch(SelectMode.Fetch, api => api.UserClaims)
                .Fetch(SelectMode.Fetch, api => api.Properties)
                // Left specification is mandatory for NHibernate to eagerly fetch the associations
                .Left.JoinQueryOver<ApiResourceScope>(api => api.Scopes)
                    .Where(s => s.Scope.IsIn(scopeNames.ToList()))
                .TransformUsing(Transformers.DistinctRootEntity);

            var results = await resourcesQuery.ListAsync();

            var models = results.Select(x => x.ToModel()).ToArray();

            _logger.LogDebug("Found {apis} API scopes in database", models.Select(x => x.Name));

            return models;
        }
        
        /// <summary>
        /// Gets identity resources by scope names.
        /// </summary>
        /// <param name="scopeNames">Scope name/names.</param>
        public async Task<IEnumerable<Models.IdentityResource>> FindIdentityResourcesByScopeNameAsync(IEnumerable<string> scopeNames)
        {
            var resourcesQuery = _session.QueryOver<Entities.IdentityResource>()
                .Fetch(SelectMode.Fetch, ir => ir.UserClaims)
                .Fetch(SelectMode.Fetch, ir => ir.Properties)
                .Where(r => r.Name.IsIn(scopeNames.ToList()))
                .TransformUsing(Transformers.DistinctRootEntity);

            var results = await resourcesQuery.ListAsync();

            _logger.LogDebug("Found {scopes} identity scopes in database", results.Select(x => x.Name));

            return results.Select(x => x.ToModel()).ToArray();
        }

        /// <summary>
        /// Gets scopes by scope name.
        /// </summary>
        /// <param name="scopeNames">Scope name/names.</param>
        public async Task<IEnumerable<Models.ApiScope>> FindApiScopesByNameAsync(IEnumerable<string> scopeNames)
        {
            var apiScopesQuery = _session.QueryOver<Entities.ApiScope>()
                .Fetch(SelectMode.Fetch, scope => scope.UserClaims)
                .Fetch(SelectMode.Fetch, scope => scope.Properties)
                .Where(scope => scope.Name.IsIn(scopeNames.ToList()));

            var results = await apiScopesQuery.ListAsync();

            _logger.LogDebug("Found {scopes} scopes in database", results.Select(x => x.Name));

            return results.Select(x => x.ToModel()).ToArray();
        }

        /// <summary>
        /// Gets all resources.
        /// </summary>
        public async Task<Resources> GetAllResourcesAsync()
        {
            Resources result;
            using (var tx = _session.BeginTransaction())
            {
                var identityResources = _session.QueryOver<Entities.IdentityResource>()
                    .Fetch(SelectMode.Fetch, ir => ir.UserClaims)
                    .Fetch(SelectMode.Fetch, ir => ir.Properties)
                    .Future();

                var apiResources = _session.QueryOver<Entities.ApiResource>()
                    .Fetch(SelectMode.Fetch, ar => ar.Secrets)
                    .Fetch(SelectMode.Fetch, ar => ar.Scopes)
                    .Fetch(SelectMode.Fetch, ar => ar.UserClaims)
                    .Fetch(SelectMode.Fetch, ar => ar.Properties)
                    .TransformUsing(Transformers.DistinctRootEntity)
                    .Future();

                var apiScopes = _session.QueryOver<Entities.ApiScope>()
                    .Fetch(SelectMode.Fetch, scope => scope.UserClaims)
                    .Fetch(SelectMode.Fetch, scope => scope.Properties)
                    .Future();

                result = new Resources(
                    (await identityResources.GetEnumerableAsync())
                        .Select(identity => identity.ToModel())
                        .ToArray(),
                    (await apiResources.GetEnumerableAsync())
                        .Select(api => api.ToModel())
                        .ToArray(),
                    (await apiScopes.GetEnumerableAsync())
                        .Select(scope => scope.ToModel())
                        .ToArray()
                );

                await tx.CommitAsync();
            }

            _logger.LogDebug("Found {scopes} as all scopes, and {apis} as API resources",
                result.IdentityResources.Select(identity => identity.Name)
                    .Union(result.ApiScopes.Select(scope => scope.Name)),
                result.ApiResources.Select(r => r.Name));

            return result;
        }
    }
}