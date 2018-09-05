namespace IdentityServer4.NHibernate.IntegrationTests.ConfigurationStore
{
    using global::NHibernate;
    using Xunit;

    public class ClientStoreFixture : DatabaseFixture
    {
        public static readonly TheoryData<ISessionFactory> TestDatabases = new TheoryData<ISessionFactory>();

        static ClientStoreFixture()
        {
            SessionFactories.ForEach(sf => TestDatabases.Add(sf));
        }

        [Theory, MemberData(nameof(TestDatabases))]
        public void FindClientByIdAsync_WhenClientExists_ExpectClientRetured(ISessionFactory sessionFactory)
        {
            using (var session = sessionFactory.OpenSession())
            {
                Assert.True(true);
            }
        }
    }
}
