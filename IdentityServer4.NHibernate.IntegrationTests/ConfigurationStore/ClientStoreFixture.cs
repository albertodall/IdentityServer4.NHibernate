namespace IdentityServer4.NHibernate.IntegrationTests.ConfigurationStore
{
    using System.Linq;
    using Extensions;
    using Options;
    using Stores;
    using IdentityServer4.Models;
    using Xunit;
    using Moq;
    using FluentAssertions;
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
        public void Should_Retrieve_Client_If_It_Exists(TestDatabase testDb)
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

            Client requestedClient = null;
            var loggerMock = new Mock<ILogger<ClientStore>>();
            using (var session = testDb.SessionFactory.OpenStatelessSession())
            {
                var store = new ClientStore(session, loggerMock.Object);
                requestedClient = store.FindClientByIdAsync(testClient.ClientId).Result;
            }

            requestedClient.Should().NotBeNull();
        }

        [Theory]
        [MemberData(nameof(TestDatabases))]
        public void Should_Not_Retrieve_Non_Existing_Client(TestDatabase testDb)
        {
            Client requestedClient = null;
            var loggerMock = new Mock<ILogger<ClientStore>>();
            using (var session = testDb.SessionFactory.OpenStatelessSession())
            {
                var store = new ClientStore(session, loggerMock.Object);
                requestedClient = store.FindClientByIdAsync("not_existing_client").Result;
            }

            requestedClient.Should().BeNull();
        }

        [Theory]
        [MemberData(nameof(TestDatabases))]
        public void Should_Retrieve_Client_With_Grant_Types(TestDatabase testDb)
        {
            var testClient = new Client()
            {
                ClientId = "test_client_with_grant_types",
                ClientName = "Test Client with Grant Types",
                AllowedGrantTypes =
                {
                    "grant_1",
                    "grant_2",
                    "grant_3",
                }
            };

            using (var session = testDb.SessionFactory.OpenSession())
            {
                var entityToSave = testClient.ToEntity();
                session.Save(entityToSave);
            }

            Client requestedClient = null;
            var loggerMock = new Mock<ILogger<ClientStore>>();
            using (var session = testDb.SessionFactory.OpenStatelessSession())
            {
                var store = new ClientStore(session, loggerMock.Object);
                requestedClient = store.FindClientByIdAsync(testClient.ClientId).Result;
            }

            requestedClient.Should().NotBeNull();
            requestedClient.AllowedGrantTypes.Count.Should().Be(3);
        }
    }
}
