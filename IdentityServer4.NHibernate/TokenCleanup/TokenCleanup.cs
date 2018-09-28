using System;
using System.Threading;
using System.Threading.Tasks;
using IdentityServer4.NHibernate.Options;
using Microsoft.Extensions.Logging;

namespace IdentityServer4.NHibernate.TokenCleanup
{  
    public class TokenCleanup
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<TokenCleanup> _logger;
        private readonly OperationalStoreOptions _options;

        private CancellationTokenSource _source;

        public TokenCleanup(IServiceProvider serviceProvider, ILogger<TokenCleanup> logger, OperationalStoreOptions options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));

            if (_options.TokenCleanupInterval < 1)
            {
                throw new ArgumentException("Token cleanup interval must be at least 1 second");
            }
            if (_options.TokenCleanupBatchSize < 1)
            {
                throw new ArgumentException("Token cleanup batch size interval must be at least 1");
            }

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public TimeSpan CleanupInterval => TimeSpan.FromSeconds(_options.TokenCleanupInterval);

        public void Start()
        {
            Start(CancellationToken.None);
        }

        public void Start(CancellationToken cancellationToken)
        {
            if (_source != null) throw new InvalidOperationException("TokenCleanup task already started. Call Stop() first.");

            _logger.LogDebug("TokenCleanup - Starting token cleanup.");

            _source = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            Task.Factory.StartNew(() => StartInternal(_source.Token));
        }

        public void Stop()
        {
            if (_source == null) throw new InvalidOperationException("TokenCleanup task not started. Call Start() first.");

            _logger.LogDebug("TokenCleanup - Stopping token cleanup.");

            _source.Cancel();
            _source = null;
        }

        private async Task StartInternal(CancellationToken cancellationToken)
        {
            while (true)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    _logger.LogDebug("TokenCleanup - CancellationRequested. Exiting.");
                    break;
                }

                try
                {
                    await Task.Delay(CleanupInterval, cancellationToken);
                }
                catch (TaskCanceledException)
                {
                    _logger.LogDebug("TokenCleanup -TaskCanceledException. Exiting.");
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError("TokenCleanup - Task.Delay exception: {0}. Exiting.", ex.Message);
                    break;
                }

                ClearTokens();
            }
        }

        /// <summary>
        /// Performs the actual token cleanup.
        /// </summary>
        public void ClearTokens()
        {
            try
            {
                _logger.LogTrace("TokenCleanup - Querying for tokens to clear");
            }
            catch (Exception ex)
            {
                _logger.LogError("TokenCleanup - Exception clearing tokens: {exception}", ex.Message);
            }
        }
    }
}
