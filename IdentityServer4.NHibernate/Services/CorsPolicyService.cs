namespace IdentityServer4.NHibernate.Services
{
    using System;
    using System.Threading.Tasks;
    using IdentityServer4.Services;

    public class CorsPolicyService : ICorsPolicyService
    {
        public Task<bool> IsOriginAllowedAsync(string origin)
        {
            throw new NotImplementedException();
        }
    }
}