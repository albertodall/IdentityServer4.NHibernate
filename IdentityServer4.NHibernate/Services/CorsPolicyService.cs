using System;
using System.Threading.Tasks;
using IdentityServer4.Services;

namespace IdentityServer4.NHibernate.Services
{
    public class CorsPolicyService : ICorsPolicyService
    {
        public Task<bool> IsOriginAllowedAsync(string origin)
        {
            throw new NotImplementedException();
        }
    }
}