using System;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.NHibernate.Entities;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NHibernate;

namespace IdentityServer4.NHibernate.Services
{
    /// <summary>
    /// Implementation of NHibernate-based CorsPolicyService.
    /// Checks the client configuration on the database for allowed CORS origins.
    /// </summary>
    public class CorsPolicyService : ICorsPolicyService
    {
        private readonly IHttpContextAccessor _context;
        private readonly ILogger<CorsPolicyService> _logger;

        /// <summary>
        /// Creates a new instance of the NHibernate-based Cors Policy Service.
        /// </summary>
        /// <param name="context">The http context.</param>
        /// <param name="logger">The logger.</param>
        public CorsPolicyService(IHttpContextAccessor context, ILogger<CorsPolicyService> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger;
        }

        /// <summary>
        /// Determines whether origin is allowed or not.
        /// </summary>
        /// <param name="origin">The origin.</param>
        public async Task<bool> IsOriginAllowedAsync(string origin)
        {
            origin = origin.ToLowerInvariant();

            bool isAllowed;
            using (var session = _context.HttpContext.RequestServices.GetRequiredService<IStatelessSession>())
            {
                var originsQuery = session.QueryOver<ClientCorsOrigin>()
                    .Where(o => o.Origin == origin)
                    .ToRowCountQuery();

                isAllowed = await originsQuery.RowCountAsync() > 0;
            }

            _logger.LogDebug("Origin {origin} is allowed: {originAllowed}", origin, isAllowed);

            return isAllowed;
        }
    }
}