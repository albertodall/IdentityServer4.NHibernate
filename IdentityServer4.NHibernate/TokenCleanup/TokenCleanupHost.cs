namespace IdentityServer4.NHibernate.TokenCleanup
{
    using System.Threading;
    using System.Threading.Tasks;
    using IdentityServer4.NHibernate.Options;
    using Microsoft.Extensions.Hosting;

    internal class TokenCleanupHost : IHostedService
    {
        private readonly TokenCleanup _tokenCleanup;
        private readonly OperationalStoreOptions _options;

        public TokenCleanupHost(TokenCleanup tokenCleanup, OperationalStoreOptions options)
        {
            _tokenCleanup = tokenCleanup;
            _options = options;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            if (_options.EnableTokenCleanup)
            {
                _tokenCleanup.Start(cancellationToken);
            }
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            if (_options.EnableTokenCleanup)
            {
                _tokenCleanup.Stop();
            }
            return Task.CompletedTask;
        }
    }
}
