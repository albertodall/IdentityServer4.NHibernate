using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.NHibernate.Extensions;
using IdentityServer4.NHibernate.IntegrationTests.TestStorage;
using IdentityServer4.NHibernate.Stores;
using Microsoft.Extensions.Logging;
using FluentAssertions;
using Moq;
using Xunit;

namespace IdentityServer4.NHibernate.IntegrationTests.ConfigurationStore
{
    public class ResourceStoreFixture : IntegrationTestFixture, IClassFixture<DatabaseFixture>
    {
        public static TheoryData<TestDatabase> TestDatabases;

        static ResourceStoreFixture()
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

        public ResourceStoreFixture(DatabaseFixture fixture)
        {
            var testDatabases = TestDatabases.SelectMany(t => t.Select(db => (TestDatabase)db)).ToList();
            fixture.TestDatabases = testDatabases;
        }

        [Theory]
        [MemberData(nameof(TestDatabases))]
        public async Task Should_Retrieve_Existing_Api_Resources_By_Scope_Names(TestDatabase testDb)
        {
            string testScope1 = "ar1_scope1";
            string testScope2 = "ar1_scope2";

            var testApiResource1 = CreateTestApiResource("test_api_resource1", new[] { testScope1, testScope2 });

            using (var session = testDb.OpenSession())
            {
                await session.SaveAsync(testApiResource1.ToEntity());
                await session.FlushAsync();
            }

            var loggerMock = new Mock<ILogger<ResourceStore>>();
            IEnumerable<ApiResource> resources;
            using (var session = testDb.OpenSession())
            {
                var store = new ResourceStore(session, loggerMock.Object);
                resources = await store.FindApiResourcesByScopeAsync(new string[] { testScope1, testScope2 });
            }

            resources.Count().Should().Be(1);
            resources.FirstOrDefault(x => x.Name == testApiResource1.Name).Should().NotBeNull();
            resources.First().Scopes.Count().Should().Be(2);
            resources.First().Scopes.First().UserClaims.Count().Should().Be(1);
            resources.First().UserClaims.Count().Should().Be(2);

            await CleanupTestDataAsync(testDb);
        }

        [Theory]
        [MemberData(nameof(TestDatabases))]
        public async Task Should_Retrieve_Requested_Api_Resources_By_Scope_Names(TestDatabase testDb)
        {
            string testScope1 = "ar2_scope1";
            string testScope2 = "ar2_scope2";

            var testApiResource1 = CreateTestApiResource("test_api_resource2", new[] { testScope1, testScope2 });
            var testApiResource2 = CreateTestApiResource("test_api_resource3", new[] { "ar3_scope3" });

            using (var session = testDb.OpenSession())
            {
                await session.SaveAsync(testApiResource1.ToEntity());
                await session.SaveAsync(testApiResource2.ToEntity());
                await session.FlushAsync();
            }

            var loggerMock = new Mock<ILogger<ResourceStore>>();
            IEnumerable<ApiResource> resources;
            using (var session = testDb.OpenSession())
            {
                var store = new ResourceStore(session, loggerMock.Object);
                resources = await store.FindApiResourcesByScopeAsync(new[] { testScope1, testScope2 });
            }

            resources.Count().Should().Be(1);
            resources.FirstOrDefault(x => x.Name == testApiResource1.Name).Should().NotBeNull();
            resources.First().Scopes.Count().Should().Be(2);
            resources.First().Scopes.First().UserClaims.Count().Should().Be(1);
            resources.First().UserClaims.Count().Should().Be(2);

            await CleanupTestDataAsync(testDb);
        }

        [Theory]
        [MemberData(nameof(TestDatabases))]
        public async Task Should_Retrieve_All_Api_Resources_By_Scope_Names(TestDatabase testDb)
        {
            string testScope1 = Guid.NewGuid().ToString();
            string testScope2 = Guid.NewGuid().ToString();
            string testScope3 = Guid.NewGuid().ToString();

            var testApiResource1 = CreateTestApiResource(Guid.NewGuid().ToString(), new[] { testScope1, testScope2 });
            var testApiResource2 = CreateTestApiResource(Guid.NewGuid().ToString(), new[] { testScope3 });

            using (var session = testDb.OpenSession())
            {
                await session.SaveAsync(testApiResource1.ToEntity());
                await session.SaveAsync(testApiResource2.ToEntity());
                await session.FlushAsync();
            }

            var loggerMock = new Mock<ILogger<ResourceStore>>();
            IEnumerable<ApiResource> resources;
            using (var session = testDb.OpenSession())
            {
                var store = new ResourceStore(session, loggerMock.Object);
                resources = await store.FindApiResourcesByScopeAsync(new[] { testScope1, testScope2, testScope3 });
            }

            resources.Count().Should().Be(2);

            await CleanupTestDataAsync(testDb);
        }

        [Theory]
        [MemberData(nameof(TestDatabases))]
        public async Task Should_Not_Retrieve_Api_Resources_With_Unexisting_Scope_Name(TestDatabase testDb)
        {
            string testScope1 = "test_api_resource_scope1";
            string testScope2 = "test_api_resource_scope2";

            var testApiResource = CreateTestApiResource("test_api_resource", new[] { testScope1, testScope2 });

            using (var session = testDb.OpenSession())
            {
                await session.SaveAsync(testApiResource.ToEntity());
                await session.FlushAsync();
            }

            var loggerMock = new Mock<ILogger<ResourceStore>>();
            IEnumerable<ApiResource> resources;
            using (var session = testDb.OpenSession())
            {
                var store = new ResourceStore(session, loggerMock.Object);
                resources = await store.FindApiResourcesByScopeAsync(new[] { "non_existing_scope" });
            }

            resources.Should().BeEmpty();

            await CleanupTestDataAsync(testDb);
        }

        [Theory]
        [MemberData(nameof(TestDatabases))]
        public async Task Should_Retrieve_Existing_Api_Resource_By_Name(TestDatabase testDb)
        {
            string testScope1 = "ar4_scope1";
            var testApiResource = CreateTestApiResource("test_api_resource4", new[] { testScope1 });

            using (var session = testDb.OpenSession())
            {
                await session.SaveAsync(testApiResource.ToEntity());
                await session.FlushAsync();
            }

            var loggerMock = new Mock<ILogger<ResourceStore>>();
            ApiResource resource;
            using (var session = testDb.OpenSession())
            {
                var store = new ResourceStore(session, loggerMock.Object);
                resource = await store.FindApiResourceAsync(testApiResource.Name);
            }

            resource.Should().NotBeNull();
            resource.ApiSecrets.Should().NotBeEmpty();
            resource.Scopes.Should().NotBeEmpty();
            resource.Scopes.Count().Should().Be(1);
            resource.Scopes.First().UserClaims.Should().NotBeEmpty();
            resource.Scopes.First().UserClaims.Count().Should().Be(1);
            resource.UserClaims.Count().Should().Be(2);

            await CleanupTestDataAsync(testDb);
        }

        [Theory]
        [MemberData(nameof(TestDatabases))]
        public async Task Should_Not_Retrieve_Non_Existing_Api_Resource(TestDatabase testDb)
        {
            var loggerMock = new Mock<ILogger<ResourceStore>>();
            ApiResource resource;
            using (var session = testDb.OpenSession())
            {
                var store = new ResourceStore(session, loggerMock.Object);
                resource = await store.FindApiResourceAsync("non_existing_api_resource");
            }

            resource.Should().BeNull();
        }

        [Theory]
        [MemberData(nameof(TestDatabases))]
        public async Task Should_Retrieve_All_Resources_Included_Hidden_Ones(TestDatabase testDb)
        {
            var visibleIdentityResource = CreateTestIdentityResource("identity_visible");
            var visibleApiResource = CreateTestApiResource("api_visible", new[] { "api_visible_scope1", "api_visible_scope_2" });
            var hiddenIdentityResource = new IdentityResource()
            {
                Name = "identity_hidden",
                ShowInDiscoveryDocument = false
            };
            var hiddenApiResource = new ApiResource()
            {
                Name = "api_hidden",
                Scopes = { new Scope("scope_hidden_1") { ShowInDiscoveryDocument = false } }
            };

            using (var session = testDb.OpenSession())
            {
                await session.SaveAsync(visibleIdentityResource.ToEntity());
                await session.SaveAsync(visibleApiResource.ToEntity());
                await session.SaveAsync(hiddenIdentityResource.ToEntity());
                await session.SaveAsync(hiddenApiResource.ToEntity());
                await session.FlushAsync();
            }

            var loggerMock = new Mock<ILogger<ResourceStore>>();
            Resources resources;
            using (var session = testDb.OpenSession())
            {
                var store = new ResourceStore(session, loggerMock.Object);
                resources = await store.GetAllResourcesAsync();
            }

            resources.Should().NotBeNull();
            resources.IdentityResources.Should().NotBeEmpty();
            resources.ApiResources.Should().NotBeEmpty();

            Assert.Contains(resources.IdentityResources, x => !x.ShowInDiscoveryDocument);
            Assert.Contains(resources.ApiResources, x => !x.Scopes.Any(y => y.ShowInDiscoveryDocument));

            await CleanupTestDataAsync(testDb);
        }

        [Theory]
        [MemberData(nameof(TestDatabases))]
        public async Task Should_Retrieve_All_Resource_With_More_Than_One_Api_Scope(TestDatabase testDb)
        {
            var testIdentityResource = CreateTestIdentityResource("identity_resource_1");
            var testApiResource = new ApiResource()
            {
                Name = "test_api_resource_1",
                ApiSecrets = new List<Secret> { new Secret("secret".ToSha256()) },
                Scopes = new List<Scope>(),
                UserClaims =
                {
                    "user_claim_1",
                    "user_claim_2"
                }
            };

            testApiResource.Scopes.Add(
                new Scope()
                {
                    Name = "user",
                    UserClaims =
                    {
                        "test_api_resource_1_user_claim_1",
                        "test_api_resource_1_user_claim_2"
                    }
                });

            using (var session = testDb.OpenSession())
            {
                await session.SaveAsync(testIdentityResource.ToEntity());
                await session.SaveAsync(testApiResource.ToEntity());
                await session.FlushAsync();
            }

            var loggerMock = new Mock<ILogger<ResourceStore>>();
            Resources resources;
            using (var session = testDb.OpenSession())
            {
                var store = new ResourceStore(session, loggerMock.Object);
                resources = await store.GetAllResourcesAsync();
            }

            resources.Should().NotBeNull();
            resources.IdentityResources.Should().NotBeEmpty();
            resources.ApiResources.Count().Should().Be(1);

            var ar = resources.ApiResources.First();
            ar.Scopes.Count().Should().Be(1);
            ar.Scopes.First().UserClaims.Count().Should().Be(2);

            await CleanupTestDataAsync(testDb);
        }

        [Theory]
        [MemberData(nameof(TestDatabases))]
        public async Task Should_Retrieve_Identity_Resource_And_Its_Claims_By_Scope(TestDatabase testDb)
        {
            var testIdentityResourceName = "idres1";
            var testIdentityResource = CreateTestIdentityResource(testIdentityResourceName);

            using (var session = testDb.OpenSession())
            {
                await session.SaveAsync(testIdentityResource.ToEntity());
                await session.FlushAsync();
            }

            var loggerMock = new Mock<ILogger<ResourceStore>>();
            IEnumerable<IdentityResource> resources;
            using (var session = testDb.OpenSession())
            {
                var store = new ResourceStore(session, loggerMock.Object);
                resources = await store.FindIdentityResourcesByScopeAsync(
                    new[] { testIdentityResourceName });
            }

            resources.Should().NotBeEmpty();
            resources.First().Name.Should().Be(testIdentityResourceName);
            resources.First().UserClaims.Count().Should().Be(2);

            await CleanupTestDataAsync(testDb);
        }

        [Theory]
        [MemberData(nameof(TestDatabases))]
        public async Task Should_Retrieve_Only_Requested_Identity_Resources(TestDatabase testDb)
        {
            var testIdentityResource1 = CreateTestIdentityResource(Guid.NewGuid().ToString());
            var testIdentityResource2 = CreateTestIdentityResource(Guid.NewGuid().ToString());

            using (var session = testDb.OpenSession())
            {
                await session.SaveAsync(testIdentityResource1.ToEntity());
                await session.SaveAsync(testIdentityResource2.ToEntity());
                await session.FlushAsync();
            }

            var loggerMock = new Mock<ILogger<ResourceStore>>();
            IEnumerable<IdentityResource> resources;
            using (var session = testDb.OpenSession())
            {
                var store = new ResourceStore(session, loggerMock.Object);
                resources = await store.FindIdentityResourcesByScopeAsync(
                    new[] { testIdentityResource1.Name });
            }

            resources.Should().NotBeEmpty();
            resources.Count().Should().Be(1);

            await CleanupTestDataAsync(testDb);
        }


        private static IdentityResource CreateTestIdentityResource(string name)
        {
            return new IdentityResource()
            {
                Name = name,
                DisplayName = "Test Identity Resource",
                Description = "Identity Resource used for testing",
                ShowInDiscoveryDocument = true,
                UserClaims =
                {
                    JwtClaimTypes.Subject,
                    JwtClaimTypes.Name,
                }
            };
        }

        private static ApiResource CreateTestApiResource(string name, string[] scopes)
        {
            var testApiResource = new ApiResource()
            {
                Name = name,
                ApiSecrets = new List<Secret> { new Secret("secret".ToSha256()) },
                Scopes = new List<Scope>(),
                UserClaims =
                {
                    "user_claim_1",
                    "user_claim_2"
                }
            };

            foreach (var scopeToAdd in scopes)
            {
                testApiResource.Scopes.Add(
                    new Scope()
                    {
                        Name = scopeToAdd,
                        UserClaims = { "test_user_claim" }
                    });
            }

            return testApiResource;
        }

        private static async Task CleanupTestDataAsync(TestDatabase db)
        {
            using (var session = db.OpenSession())
            {
                using (var tx = session.BeginTransaction())
                {
                    await session.DeleteAsync("from IdentityResource ir");
                    await session.DeleteAsync("from ApiResource ar");
                    await tx.CommitAsync();
                }
            }
        }
    }
}
