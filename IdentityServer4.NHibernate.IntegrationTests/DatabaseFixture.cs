namespace IdentityServer4.NHibernate.IntegrationTests
{
    using System;
    using Database;
    using Extensions;
    using Options;
    using Xunit;

    public class DatabaseFixture
    {
        private static readonly ConfigurationStoreOptions StoreOptions = new ConfigurationStoreOptions();

        [Fact]
        public void Should_Create_Database()
        {
            var dbConfig = Databases.SqlServer2012()
                .UsingConnectionString("Data Source=localhost; Initial Catalog=idsNHTest; Integrated Security=SSPI")
                .AddConfigurationStoreMappings(StoreOptions);

            var sessionFactory = dbConfig.BuildSessionFactory();

            Assert.True(true);

            sessionFactory.Dispose();
        }
    }
}
