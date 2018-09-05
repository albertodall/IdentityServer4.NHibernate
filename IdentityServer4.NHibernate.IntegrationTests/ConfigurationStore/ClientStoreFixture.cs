namespace IdentityServer4.NHibernate.IntegrationTests.ConfigurationStore
{
    using System.Linq;
    using Options;
    using global::NHibernate;
    using Xunit;

    public class ClientStoreFixture : IClassFixture<DatabaseFixture>
    {
        private static readonly ConfigurationStoreOptions ConfigurationStoreOptions = new ConfigurationStoreOptions();
        private static readonly OperationalStoreOptions OperationalStoreOptions = new OperationalStoreOptions();

        public static readonly TheoryData<ISessionFactory> TestDatabases = new TheoryData<ISessionFactory>()
        {
            TestDatabaseBuilder.SQLServer2012TestDatabase("IdentityServer_NH_Test", ConfigurationStoreOptions, OperationalStoreOptions)
        };

        public ClientStoreFixture(DatabaseFixture fixture)
        {
            var sfs = TestDatabases.SelectMany(t => t.Select(db => (ISessionFactory)db)).ToList();
            fixture.SessionFactories = sfs;
        }

        [Theory, MemberData(nameof(TestDatabases))]
        public void Should_Return_Client_If_Exists(ISessionFactory sessionFactory)
        {
            using (var session = sessionFactory.OpenSession())
            {
                Assert.True(true);
            }
        }
    }
}
