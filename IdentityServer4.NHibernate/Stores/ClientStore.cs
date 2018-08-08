namespace IdentityServer4.NHibernate.Stores
{
    using System;
    using System.Threading.Tasks;
    using IdentityServer4.Models;
    using IdentityServer4.Stores;

    public class ClientStore : IClientStore
    {
        public Task<Client> FindClientByIdAsync(string clientId)
        {
            throw new NotImplementedException();
        }
    }
}
