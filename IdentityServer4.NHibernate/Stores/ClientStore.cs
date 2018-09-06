namespace IdentityServer4.NHibernate.Stores
{
    using System;
    using System.Threading.Tasks;
    using Extensions;
    using IdentityServer4.Models;
    using IdentityServer4.Stores;
    using Microsoft.Extensions.Logging;
    using global::NHibernate;

    public class ClientStore : IClientStore
    {
        private readonly IStatelessSession _session;
        private readonly ILogger<ClientStore> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientStore"/> class.
        /// </summary>
        /// <param name="session">The NHibernate session used to retrieve the data.</param>
        /// <param name="logger">The logger.</param>
        /// <exception cref="ArgumentNullException">session</exception>
        public ClientStore(IStatelessSession session, ILogger<ClientStore> logger)
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
        public async Task<Client> FindClientByIdAsync(string clientId)
        {
            Entities.Client client = null;
            using (var tx = _session.BeginTransaction())
            {
                client = await _session.QueryOver<Entities.Client>()
                    .Where(c => c.ClientId == clientId)
                    .SingleOrDefaultAsync();
                await tx.CommitAsync();
            }

            Client clientModel = client.ToModel();

            _logger.LogDebug("{clientId} found in database: {clientIdFound}", clientId, clientModel != null);

            return clientModel;
        }
    }
}
