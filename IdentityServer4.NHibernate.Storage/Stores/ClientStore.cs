using System;
using System.Threading.Tasks;
using IdentityServer4.NHibernate.Entities;
using IdentityServer4.NHibernate.Extensions;
using IdentityServer4.Stores;
using Microsoft.Extensions.Logging;
using NHibernate;
using NHibernate.Transform;

namespace IdentityServer4.NHibernate.Stores
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
            Client client;
            using (var tx = _session.BeginTransaction())
            {
                var clientQuery = _session.QueryOver<Client>()
                    .Where(c => c.ClientId == clientId)
                    .Fetch(SelectMode.Fetch, c => c.AllowedGrantTypes)
                    .Fetch(SelectMode.Fetch, c => c.ClientSecrets)
                    .Fetch(SelectMode.Fetch, c => c.AllowedScopes)
                    .Fetch(SelectMode.Fetch, c => c.Claims)
                    .TransformUsing(Transformers.DistinctRootEntity)
                    .FutureValue<Client>();

                _session.QueryOver<Client>()
                    .Where(c => c.ClientId == clientId)
                    .Fetch(SelectMode.Fetch, c => c.RedirectUris)
                    .Fetch(SelectMode.Fetch, c => c.PostLogoutRedirectUris)
                    .Fetch(SelectMode.Fetch, c => c.AllowedCorsOrigins)
                    .TransformUsing(Transformers.DistinctRootEntity)
                    .FutureValue<Client>();

                _session.QueryOver<Client>()
                    .Where(c => c.ClientId == clientId)
                    .Fetch(SelectMode.Fetch, c => c.IdentityProviderRestrictions)
                    .Fetch(SelectMode.Fetch, c => c.Properties)
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
