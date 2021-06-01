using IdentityServer4.NHibernate.Options;
using Microsoft.Extensions.Configuration;

namespace IdentityServer4.NHibernate.IntegrationTests
{
    public abstract class IntegrationTestFixture
    {
        private const string TestSettingsFile = "test-database-settings.json";

        private static IConfigurationRoot TestSettings { get; }

        protected static readonly ConfigurationStoreOptions TestConfigurationStoreOptions = new ConfigurationStoreOptions();
        protected static readonly OperationalStoreOptions TestOperationalStoreOptions = new OperationalStoreOptions();

        static IntegrationTestFixture()
        {
            TestSettings = new ConfigurationBuilder()
                .AddJsonFile(TestSettingsFile)
                .Build();
        }

        protected static string SQLServerConnectionString => TestSettings["SQLServer"];
        protected static string PostgreSQLConnectionString => TestSettings["PostgreSQL"];
        protected static string MySQLConnectionString => TestSettings["MySQL"];
    }
}
