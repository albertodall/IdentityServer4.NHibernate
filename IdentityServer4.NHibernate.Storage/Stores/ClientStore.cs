using System;
using System.Threading.Tasks;
using IdentityServer4.NHibernate.Storage.Entities;
using IdentityServer4.NHibernate.Storage.Extensions;
using IdentityServer4.Stores;
using Microsoft.Extensions.Logging;
using NHibernate;
using NHibernate.Transform;

namespace IdentityServer4.NHibernate.Storage.Stores
{
    /// <summary>
    /// Implementation of the NHibernate-based client store.
    /// </summary>
    public class ClientStore : IClientStore
    {
        private readonly ISession _session;
        private readonly ILogger<ClientStore> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientStore"/> class.
        /// </summary>
        /// <param name="session">The NHibernate session used to retrieve the data.</param>
        /// <param name="logger">The logger.</param>
        /// <exception cref="ArgumentNullException">session</exception>
        public ClientStore(ISession session, ILogger<ClientStore> logger)
        {
            _session = session ?? throw new ArgumentNullException(nameof(session));
            _logger = logger;
        }

        /// <summary>
        /// Finds a client by its client id
        /// </summary>
        /// <param name="clientId">The client id</param>
        /// <returns>
        /// The client
        /// </returns>
        public async Task<Models.Client> FindClientByIdAsync(string clientId)
        {
            Client client = null;
            using (var tx = _session.BeginTransaction())
            {
                var clientQuery = _session.QueryOver<Client>()
                    .Where(c => c.ClientId == clientId)
                    .Fetch(c => c.AllowedGrantTypes).Eager
                    .Fetch(c => c.ClientSecrets).Eager
                    .Fetch(c => c.AllowedScopes).Eager
                    .Fetch(c => c.Claims).Eager
                    .TransformUsing(Transformers.DistinctRootEntity)
                    .FutureValue<Client>();

                _session.QueryOver<Client>()
                    .Where(c => c.ClientId == clientId)
                    .Fetch(c => c.RedirectUris).Eager
                    .Fetch(c => c.PostLogoutRedirectUris).Eager
                    .Fetch(c => c.AllowedCorsOrigins).Eager
                    .TransformUsing(Transformers.DistinctRootEntity)
                    .FutureValue<Client>();

                _session.QueryOver<Client>()
                    .Where(c => c.ClientId == clientId)
                    .Fetch(c => c.IdentityProviderRestrictions).Eager
                    .Fetch(c => c.Properties).Eager
                    .TransformUsing(Transformers.DistinctRootEntity)
                    .FutureValue<Client>();

                client = await clientQuery.GetValueAsync();

                await tx.CommitAsync();
            }

            Models.Client clientModel = client.ToModel();

            _logger.LogDebug("{clientId} found in database: {clientIdFound}", clientId, clientModel != null);

            return clientModel;
        }
    }
}
