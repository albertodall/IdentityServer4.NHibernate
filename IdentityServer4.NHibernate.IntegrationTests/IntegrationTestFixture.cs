using IdentityServer4.NHibernate.Options;
using Microsoft.Extensions.Configuration;

namespace IdentityServer4.NHibernate.IntegrationTests
{
    public abstract class IntegrationTestFixture
    {
        private const string TestSettingsFile = "test-database-settings.json";

        protected static readonly ConfigurationStoreOptions TestConfigurationStoreOptions = new ConfigurationStoreOptions();
        protected static readonly OperationalStoreOptions TestOperationalStoreOptions = new OperationalStoreOptions();

        protected static IConfigurationRoot TestSettings { get; }

        static IntegrationTestFixture()
        {
            TestSettings = new ConfigurationBuilder()
                .AddJsonFile(TestSettingsFile)
                .Build();
        }
    }
}
