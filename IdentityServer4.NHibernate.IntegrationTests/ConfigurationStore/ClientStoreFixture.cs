using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using IdentityServer4.NHibernate.Extensions;
using IdentityServer4.NHibernate.IntegrationTests.TestStorage;
using IdentityServer4.NHibernate.Stores;
using IdentityServer4.Models;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace IdentityServer4.NHibernate.IntegrationTests.ConfigurationStore
{
    public class ClientStoreFixture : IntegrationTestFixture, IClassFixture<DatabaseFixture>
    {
        public static TheoryData<TestDatabase> TestDatabases;

        static ClientStoreFixture()
        {
            TestDatabases = new TheoryData<TestDatabase>()
            {
                TestDatabaseBuilder.SQLServer2012TestDatabase(SQLServerConnectionString, $"{MethodBase.GetCurrentMethod()?.DeclaringType?.Name}_NH_Test", TestConfigurationStoreOptions, TestOperationalStoreOptions),
                TestDatabaseBuilder.SQLiteTestDatabase($"{MethodBase.GetCurrentMethod()?.DeclaringType?.Name}_NH_Test.sqlite", TestConfigurationStoreOptions, TestOperationalStoreOptions),
                TestDatabaseBuilder.SQLiteInMemoryTestDatabase(TestConfigurationStoreOptions, TestOperationalStoreOptions),
                TestDatabaseBuilder.PostgreSQLTestDatabase(PostgreSQLConnectionString, $"{MethodBase.GetCurrentMethod()?.DeclaringType?.Name}_NH_Test", TestConfigurationStoreOptions, TestOperationalStoreOptions),
                TestDatabaseBuilder.MySQLTestDatabase(MySQLConnectionString, $"{MethodBase.GetCurrentMethod()?.DeclaringType?.Name}_NH_Test", TestConfigurationStoreOptions, TestOperationalStoreOptions)
            };
        }

        public ClientStoreFixture(DatabaseFixture fixture)
        {
            var testDatabases = TestDatabases.SelectMany(t => t.Select(db => (TestDatabase)db)).ToList();
            fixture.TestDatabases = testDatabases;
        }

        [Theory]
        [MemberData(nameof(TestDatabases))]
        public async Task Should_Retrieve_Client_If_It_Exists(TestDatabase testDb)
        {
            var testClient = new Client()
            {
                ClientId = Guid.NewGuid().ToString(),
                ClientName = "Test Client"
            };

            using (var session = testDb.OpenSession())
            {
                await session.SaveAsync(testClient.ToEntity());
                await session.FlushAsync();
            }

            Client requestedClient;
            var loggerMock = new Mock<ILogger<ClientStore>>();
            using (var session = testDb.OpenSession())
            {
                var store = new ClientStore(session, loggerMock.Object);
                requestedClient = await store.FindClientByIdAsync(testClient.ClientId);
            }

            requestedClient.Should().NotBeNull();

            await CleanupTestDataAsync(testDb);
        }

        [Theory]
        [MemberData(nameof(TestDatabases))]
        public async Task Should_Not_Retrieve_Non_Existing_Client(TestDatabase testDb)
        {
            Client requestedClient;
            var loggerMock = new Mock<ILogger<ClientStore>>();
            using (var session = testDb.OpenSession())
            {
                var store = new ClientStore(session, loggerMock.Object);
                requestedClient = await store.FindClientByIdAsync("not_existing_client");
            }

            requestedClient.Should().BeNull();

            await CleanupTestDataAsync(testDb);
        }

        [Theory]
        [MemberData(nameof(TestDatabases))]
        public async Task Should_Retrieve_Client_With_Grant_Types(TestDatabase testDb)
        {
            var testClient = new Client()
            {
                ClientId = Guid.NewGuid().ToString(),
                ClientName = "Test Client with Grant Types",
                AllowedGrantTypes =
                {
                    "grant_1",
                    "grant_2",
                    "grant_3",
                }
            };

            using (var session = testDb.OpenSession())
            {
                var entityToSave = testClient.ToEntity();
                await session.SaveAsync(entityToSave);
                await session.FlushAsync();
            }

            Client requestedClient;
            var loggerMock = new Mock<ILogger<ClientStore>>();
            using (var session = testDb.OpenSession())
            {
                var store = new ClientStore(session, loggerMock.Object);
                requestedClient = await store.FindClientByIdAsync(testClient.ClientId);
            }

            requestedClient.Should().NotBeNull();
            requestedClient.AllowedGrantTypes.Count.Should().Be(3);

            await CleanupTestDataAsync(testDb);
        }

        [Theory]
        [MemberData(nameof(TestDatabases))]
        public async Task Should_Retrieve_Client_With_Client_Secrets(TestDatabase testDb)
        {
            var testClient = new Client()
            {
                ClientId = Guid.NewGuid().ToString(),
                ClientName = "Test Client with Client Secrets",
                ClientSecrets = new List<Secret>()
                {
                    new Secret("secret1", "secret 1"),
                    new Secret("secret2", "secret 2"),
                    new Secret("secret3", "secret 3"),
                }
            };

            using (var session = testDb.OpenSession())
            {
                var entityToSave = testClient.ToEntity();
                await session.SaveAsync(entityToSave);
                await session.FlushAsync();
            }

            Client requestedClient;
            var loggerMock = new Mock<ILogger<ClientStore>>();
            using (var session = testDb.OpenSession())
            {
                var store = new ClientStore(session, loggerMock.Object);
                requestedClient = await store.FindClientByIdAsync(testClient.ClientId);
            }

            requestedClient.Should().NotBeNull();
            requestedClient.ClientSecrets.Count.Should().Be(3);

            await CleanupTestDataAsync(testDb);
        }

        [Theory]
        [MemberData(nameof(TestDatabases))]
        public async Task Should_Retrieve_Client_With_Redirect_Uris(TestDatabase testDb)
        {
            var testClient = new Client()
            {
                ClientId = Guid.NewGuid().ToString(),
                ClientName = "Test Client with Redirect Uris",
                RedirectUris =
                {
                    @"http://redirect/uri/1",
                    @"http://redirect/uri/2",
                    @"http://redirect/uri/3"
                }
            };

            using (var session = testDb.OpenSession())
            {
                var entityToSave = testClient.ToEntity();
                await session.SaveAsync(entityToSave);
                await session.FlushAsync();
            }

            Client requestedClient;
            var loggerMock = new Mock<ILogger<ClientStore>>();
            using (var session = testDb.OpenSession())
            {
                var store = new ClientStore(session, loggerMock.Object);
                requestedClient = await store.FindClientByIdAsync(testClient.ClientId);
            }

            requestedClient.Should().NotBeNull();
            requestedClient.RedirectUris.Count.Should().Be(3);

            await CleanupTestDataAsync(testDb);
        }

        [Theory]
        [MemberData(nameof(TestDatabases))]
        public async Task Should_Retrieve_Client_With_PostLogout_Redirect_Uris(TestDatabase testDb)
        {
            var testClient = new Client()
            {
                ClientId = Guid.NewGuid().ToString(),
                ClientName = "Test Client with PostLogout Redirect Uris",
                PostLogoutRedirectUris =
                {
                    @"http://postlogout/redirect/uri/1",
                    @"http://postlogout/redirect/uri/2",
                    @"http://postlogout/redirect/uri/3"
                }
            };

            using (var session = testDb.OpenSession())
            {
                var entityToSave = testClient.ToEntity();
                await session.SaveAsync(entityToSave);
                await session.FlushAsync();
            }

            Client requestedClient;
            var loggerMock = new Mock<ILogger<ClientStore>>();
            using (var session = testDb.OpenSession())
            {
                var store = new ClientStore(session, loggerMock.Object);
                requestedClient = await store.FindClientByIdAsync(testClient.ClientId);
            }

            requestedClient.Should().NotBeNull();
            requestedClient.PostLogoutRedirectUris.Count.Should().Be(3);

            await CleanupTestDataAsync(testDb);
        }

        [Theory]
        [MemberData(nameof(TestDatabases))]
        public async Task Should_Retrieve_Client_With_Allowed_Scopes(TestDatabase testDb)
        {
            var testClient = new Client()
            {
                ClientId = Guid.NewGuid().ToString(),
                ClientName = "Test Client with Allowed Scopes",
                AllowedScopes =
                {
                    "scope1",
                    "scope2",
                    "scope3"
                }
            };

            using (var session = testDb.OpenSession())
            {
                var entityToSave = testClient.ToEntity();
                await session.SaveAsync(entityToSave);
                await session.FlushAsync();
            }

            Client requestedClient;
            var loggerMock = new Mock<ILogger<ClientStore>>();
            using (var session = testDb.OpenSession())
            {
                var store = new ClientStore(session, loggerMock.Object);
                requestedClient = await store.FindClientByIdAsync(testClient.ClientId);
            }

            requestedClient.Should().NotBeNull();
            requestedClient.AllowedScopes.Count.Should().Be(3);

            await CleanupTestDataAsync(testDb);
        }

        [Theory]
        [MemberData(nameof(TestDatabases))]
        public async Task Should_Retrieve_Client_With_Provider_Restrictions(TestDatabase testDb)
        {
            var testClient = new Client()
            {
                ClientId = Guid.NewGuid().ToString(),
                ClientName = "Test Client with Provider Restrictions",
                IdentityProviderRestrictions =
                {
                    "restriction_1",
                    "restriction_2",
                    "restriction_3"
                }
            };

            using (var session = testDb.OpenSession())
            {
                var entityToSave = testClient.ToEntity();
                await session.SaveAsync(entityToSave);
                await session.FlushAsync();
            }

            Client requestedClient;
            var loggerMock = new Mock<ILogger<ClientStore>>();
            using (var session = testDb.OpenSession())
            {
                var store = new ClientStore(session, loggerMock.Object);
                requestedClient = await store.FindClientByIdAsync(testClient.ClientId);
            }

            requestedClient.Should().NotBeNull();
            requestedClient.IdentityProviderRestrictions.Count.Should().Be(3);

            await CleanupTestDataAsync(testDb);
        }

        [Theory]
        [MemberData(nameof(TestDatabases))]
        public async Task Should_Retrieve_Client_With_Claims(TestDatabase testDb)
        {
            var testClient = new Client()
            {
                ClientId = Guid.NewGuid().ToString(),
                ClientName = "Test Client with Claims",
                Claims =
                {
                    new ClientClaim("type1", "value1"),
                    new ClientClaim("type2", "value2"),
                    new ClientClaim("type3", "value3")
                }
            };

            using (var session = testDb.OpenSession())
            {
                var entityToSave = testClient.ToEntity();
                await session.SaveAsync(entityToSave);
                await session.FlushAsync();
            }

            Client requestedClient;
            var loggerMock = new Mock<ILogger<ClientStore>>();
            using (var session = testDb.OpenSession())
            {
                var store = new ClientStore(session, loggerMock.Object);
                requestedClient = await store.FindClientByIdAsync(testClient.ClientId);
            }

            requestedClient.Should().NotBeNull();
            requestedClient.Claims.Count.Should().Be(3);

            await CleanupTestDataAsync(testDb);
        }

        [Theory]
        [MemberData(nameof(TestDatabases))]
        public async Task Should_Retrieve_Client_With_Allowed_Cors_Origins(TestDatabase testDb)
        {
            var testClient = new Client()
            {
                ClientId = Guid.NewGuid().ToString(),
                ClientName = "Test Client with CORS Origins",
                AllowedCorsOrigins =
                {
                    "*.tld1",
                    "*.tld2",
                    "*.tld3"
                }
            };

            using (var session = testDb.OpenSession())
            {
                await session.SaveAsync(testClient.ToEntity());
                await session.FlushAsync();
            }

            Client requestedClient;
            var loggerMock = new Mock<ILogger<ClientStore>>();
            using (var session = testDb.OpenSession())
            {
                var store = new ClientStore(session, loggerMock.Object);
                requestedClient = await store.FindClientByIdAsync(testClient.ClientId);
            }

            requestedClient.Should().NotBeNull();
            requestedClient.AllowedCorsOrigins.Count.Should().Be(3);

            await CleanupTestDataAsync(testDb);
        }

        [Theory]
        [MemberData(nameof(TestDatabases))]
        public async Task Should_Retrieve_Client_With_Properties(TestDatabase testDb)
        {
            var testClient = new Client()
            {
                ClientId = Guid.NewGuid().ToString(),
                ClientName = "Test Client with Properties",
                Properties =
                {
                    { "prop1", "val1" },
                    { "prop2", "val2" },
                    { "prop3", "val3" }
                }
            };

            using (var session = testDb.OpenSession())
            {
                await session.SaveAsync(testClient.ToEntity());
                await session.FlushAsync();
            }

            Client requestedClient;
            var loggerMock = new Mock<ILogger<ClientStore>>();
            using (var session = testDb.OpenSession())
            {
                var store = new ClientStore(session, loggerMock.Object);
                requestedClient = await store.FindClientByIdAsync(testClient.ClientId);
            }

            requestedClient.Should().NotBeNull();
            requestedClient.Properties.Count.Should().Be(3);

            await CleanupTestDataAsync(testDb);
        }

        private static async Task CleanupTestDataAsync(TestDatabase db)
        {
            using (var session = db.OpenSession())
            {
                using (var tx = session.BeginTransaction())
                {
                    await session.DeleteAsync("from Client c");
                    await tx.CommitAsync();
                }
            }
        }
    }
}
