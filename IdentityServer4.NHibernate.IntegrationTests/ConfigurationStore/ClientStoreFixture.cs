namespace IdentityServer4.NHibernate.IntegrationTests.ConfigurationStore
{
    using System.Linq;
    using Extensions;
    using Options;
    using Stores;
    using IdentityServer4.Models;
    using Xunit;
    using Moq;
    using Microsoft.Extensions.Logging;

    public class ClientStoreFixture : IClassFixture<DatabaseFixture>
    {
        private static readonly ConfigurationStoreOptions ConfigurationStoreOptions = new ConfigurationStoreOptions();
        private static readonly OperationalStoreOptions OperationalStoreOptions = new OperationalStoreOptions();

        public static readonly TheoryData<TestDatabase> TestDatabases = new TheoryData<TestDatabase>()
        {
            TestDatabaseBuilder.SQLServer2012TestDatabase("(local)", "IdentityServer_NH_Test", ConfigurationStoreOptions, OperationalStoreOptions)
        };

        public ClientStoreFixture(DatabaseFixture fixture)
        {
            var testDatabases = TestDatabases.SelectMany(t => t.Select(db => (TestDatabase)db)).ToList();
            fixture.TestDatabases = testDatabases;
        }

        [Theory]
        [MemberData(nameof(TestDatabases))]
        public void Should_Return_Client_If_It_Exists(TestDatabase testDb)
        {
            var testClient = new Client()
            {
                ClientId = "test_client",
                ClientName = "Test Client"
            };

            using (var session = testDb.SessionFactory.OpenSession())
            {
                session.Save(testClient.ToEntity());
            }

            Client wantedClient = null;
            var loggerMock = new Mock<ILogger<ClientStore>>();
            using (var session = testDb.SessionFactory.OpenStatelessSession())
            {
                var store = new ClientStore(session, loggerMock.Object);
                wantedClient = store.FindClientByIdAsync("test_client").Result;
            }

            Assert.NotNull(wantedClient);
        }
    }
}
