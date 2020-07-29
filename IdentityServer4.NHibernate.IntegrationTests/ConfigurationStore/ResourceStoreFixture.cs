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
            var sqlServerDataSource = TestSettings["SQLServer"];

            TestDatabases = new TheoryData<TestDatabase>()
            {
                TestDatabaseBuilder.SQLServer2012TestDatabase(sqlServerDataSource, $"{MethodBase.GetCurrentMethod()?.DeclaringType?.Name}_NH_Test", TestConfigurationStoreOptions, TestOperationalStoreOptions),
                TestDatabaseBuilder.SQLiteTestDatabase($"{MethodBase.GetCurrentMethod()?.DeclaringType?.Name}_NH_Test.sqlite", TestConfigurationStoreOptions, TestOperationalStoreOptions),
                TestDatabaseBuilder.SQLiteInMemoryTestDatabase(TestConfigurationStoreOptions, TestOperationalStoreOptions)
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
            var testApiResource1 = CreateTestApiResource("test_api_resource1", new [] { "ar1_userclaim1", "ar1_userclaim2" });
            var testApiScope1 = CreateTestApiScopeForResource("test_api_scope1", new[] { "as_user_scope1" });
            testApiResource1.Scopes.Add(testApiScope1.Name);

            using (var session = testDb.OpenSession())
            {
                await session.SaveAsync(testApiResource1.ToEntity());
                await session.SaveAsync(testApiScope1.ToEntity());
                await session.FlushAsync();
            }

            var loggerMock = new Mock<ILogger<ResourceStore>>();
            IEnumerable<ApiResource> resources;
            using (var session = testDb.OpenStatelessSession())
            {
                var store = new ResourceStore(session, loggerMock.Object);
                resources = (await store.FindApiResourcesByScopeNameAsync(new[] { testApiScope1.Name })).ToList();
            }

            resources.Count().Should().Be(1);
            resources.First().Name.Should().Be("test_api_resource1");
            resources.First().Scopes.Count.Should().Be(1);
            resources.First().Scopes.First().Should().Be("test_api_scope1");

            await CleanupTestDataAsync(testDb);
        }

        [Theory]
        [MemberData(nameof(TestDatabases))]
        public async Task Should_Retrieve_Requested_Api_Resources_By_Scope_Names(TestDatabase testDb)
        {
            const string testScope1 = "ar2_scope1";
            const string testScope2 = "ar2_scope2";
            const string testScope3 = "ar2_scope3";

            var testApiResource1 = CreateTestApiResource("test_api_resource2", new[] { testScope1, testScope2 });
            var testApiScope1 = CreateTestApiScopeForResource(testScope1, new[] { "u_scope3", "u_scope4" });
            var testApiScope2 = CreateTestApiScopeForResource(testScope2, new[] { "u_scope5", "u_scope6" });
            testApiResource1.Scopes.Add(testApiScope1.Name);
            testApiResource1.Scopes.Add(testApiScope2.Name);

            var testApiResource2 = CreateTestApiResource("test_api_resource3", new[] { testScope3 });
            var testApiScope3 = CreateTestApiScopeForResource(testScope3, new[] { "u_scope7", "u_scope8" });
            testApiResource2.Scopes.Add(testApiScope3.Name);

            using (var session = testDb.OpenSession())
            {
                await session.SaveAsync(testApiResource1.ToEntity());
                await session.SaveAsync(testApiResource2.ToEntity());
                await session.SaveAsync(testApiScope1.ToEntity());
                await session.SaveAsync(testApiScope2.ToEntity());
                await session.SaveAsync(testApiScope3.ToEntity());
                await session.FlushAsync();
            }

            var loggerMock = new Mock<ILogger<ResourceStore>>();
            IEnumerable<ApiResource> resources;
            using (var session = testDb.OpenStatelessSession())
            {
                var store = new ResourceStore(session, loggerMock.Object);
                resources = (await store.FindApiResourcesByScopeNameAsync(new[] { testScope1, testScope2 })).ToList();
            }

            resources.Count().Should().Be(1);
            resources.First().Scopes.Count.Should().Be(2);

            await CleanupTestDataAsync(testDb);
        }

        [Theory]
        [MemberData(nameof(TestDatabases))]
        public async Task Should_Retrieve_All_Api_Resources_By_Scope_Names(TestDatabase testDb)
        {
            var testScope1 = Guid.NewGuid().ToString();
            var testScope2 = Guid.NewGuid().ToString();
            var testScope3 = Guid.NewGuid().ToString();

            var testApiResource1 = CreateTestApiResource(Guid.NewGuid().ToString(), new[] { "user_claim1", "user_claim2" });
            var testApiScope1 = CreateTestApiScopeForResource(testScope1, new[] { "as1_user_claim1" });
            var testApiScope2 = CreateTestApiScopeForResource(testScope2, new[] { "as2_user_claim1" });
            testApiResource1.Scopes.Add(testApiScope1.Name);
            testApiResource1.Scopes.Add(testApiScope2.Name);
            var testApiResource2 = CreateTestApiResource(Guid.NewGuid().ToString(), new[] { "user_claim3" });
            var testApiScope3 = CreateTestApiScopeForResource(testScope3, new[] { "as3_user_claim1" });
            testApiResource2.Scopes.Add(testApiScope3.Name);

            using (var session = testDb.OpenSession())
            {
                await session.SaveAsync(testApiResource1.ToEntity());
                await session.SaveAsync(testApiResource2.ToEntity());
                await session.SaveAsync(testApiScope1.ToEntity());
                await session.SaveAsync(testApiScope2.ToEntity());
                await session.SaveAsync(testApiScope3.ToEntity());
                await session.FlushAsync();
            }

            var loggerMock = new Mock<ILogger<ResourceStore>>();
            IEnumerable<ApiResource> resources;
            using (var session = testDb.OpenStatelessSession())
            {
                var store = new ResourceStore(session, loggerMock.Object);
                resources = await store.FindApiResourcesByScopeNameAsync(new[] { testScope1, testScope2, testScope3 });
            }

            resources.ToList().Count.Should().Be(2);

            await CleanupTestDataAsync(testDb);
        }

        [Theory]
        [MemberData(nameof(TestDatabases))]
        public async Task Should_Not_Retrieve_Api_Resources_With_Unexisting_Scope_Name(TestDatabase testDb)
        {
            var testApiResource = CreateTestApiResource("test_api_resource", new[] { "ar_userclaim" });

            using (var session = testDb.OpenSession())
            {
                await session.SaveAsync(testApiResource.ToEntity());
                await session.FlushAsync();
            }

            var loggerMock = new Mock<ILogger<ResourceStore>>();
            IEnumerable<ApiResource> resources;
            using (var session = testDb.OpenStatelessSession())
            {
                var store = new ResourceStore(session, loggerMock.Object);
                resources = await store.FindApiResourcesByScopeNameAsync(new[] {"non_existing_scope"});
            }

            resources.ToList().Should().BeEmpty();

            await CleanupTestDataAsync(testDb);
        }

        [Theory]
        [MemberData(nameof(TestDatabases))]
        public async Task Should_Retrieve_Existing_Api_Resource_By_Name(TestDatabase testDb)
        {
            var testApiResource = CreateTestApiResource("test_api_resource4", new[] { "ar4_user_claim1" });
            var testApiScope = CreateTestApiScopeForResource("ar4_scope", new[] { "ar4scope_user_claim" });
            testApiResource.Scopes.Add(testApiScope.Name);

            using (var session = testDb.OpenSession())
            {
                await session.SaveAsync(testApiResource.ToEntity());
                await session.SaveAsync(testApiScope.ToEntity());
                await session.FlushAsync();
            }

            var loggerMock = new Mock<ILogger<ResourceStore>>();
            ApiResource resource;
            using (var session = testDb.OpenStatelessSession())
            {
                var store = new ResourceStore(session, loggerMock.Object);
                resource = (await store.FindApiResourcesByNameAsync(new[] { testApiResource.Name })).SingleOrDefault();
            }

            resource.Should().NotBeNull();
            resource?.Name.Should().Be("test_api_resource4");
            resource?.ApiSecrets.Should().NotBeEmpty();
            resource?.Scopes.Should().NotBeEmpty();
            resource?.Scopes.Count.Should().Be(1);

            await CleanupTestDataAsync(testDb);
        }

        [Theory]
        [MemberData(nameof(TestDatabases))]
        public async Task Should_Retrieve_Requested_Api_Resource_By_Name(TestDatabase testDb)
        {
            var testApiResource1 = CreateTestApiResource("test_api_resource5", new[] { "ar5_user_scope1" });
            var testApiResource2 = CreateTestApiResource("test_api_resource6", new[] { "ar6_user_scope1" });

            using (var session = testDb.OpenSession())
            {
                await session.SaveAsync(testApiResource1.ToEntity());
                await session.SaveAsync(testApiResource2.ToEntity());
                await session.FlushAsync();
            }

            var loggerMock = new Mock<ILogger<ResourceStore>>();
            IEnumerable<ApiResource> resources;
            using (var session = testDb.OpenStatelessSession())
            {
                var store = new ResourceStore(session, loggerMock.Object);
                resources = (await store.FindApiResourcesByNameAsync(new[] { testApiResource1.Name })).ToList();
            }

            resources.Count().Should().Be(1);
            resources.First().Name.Should().Be("test_api_resource5");

            await CleanupTestDataAsync(testDb);
        }

        [Theory]
        [MemberData(nameof(TestDatabases))]
        public async Task Should_Not_Retrieve_Non_Existing_Api_Resource(TestDatabase testDb)
        {
            var loggerMock = new Mock<ILogger<ResourceStore>>();
            ApiResource resource;
            using (var session = testDb.OpenStatelessSession())
            {
                var store = new ResourceStore(session, loggerMock.Object);
                resource = (await store.FindApiResourcesByNameAsync(new [] { "non_existing_api_resource" })).SingleOrDefault();
            }

            resource.Should().BeNull();
        }

        [Theory]
        [MemberData(nameof(TestDatabases))]
        public async Task Should_Retrieve_All_Resources_Including_Hidden_Ones(TestDatabase testDb)
        {
            var visibleIdentityResource = CreateTestIdentityResource("identity_visible");
            var visibleApiResource = CreateTestApiResource("api_visible", new[] { Guid.NewGuid().ToString() });
            var visibleApiScope = CreateTestApiScopeForResource("api_visible_scope", new[] { Guid.NewGuid().ToString() });
            visibleApiResource.Scopes.Add(visibleApiScope.Name);
            var hiddenIdentityResource = new IdentityResource()
            {
                Name = "identity_hidden",
                ShowInDiscoveryDocument = false
            };
            var hiddenApiResource = new ApiResource()
            {
                Name = "api_hidden",
                Scopes = { Guid.NewGuid().ToString() },
                ShowInDiscoveryDocument = false
            };
            var hiddenApiScope = new ApiScope()
            {
                Name = "api_scope_hidden",
                ShowInDiscoveryDocument = false
            };

            using (var session = testDb.OpenSession())
            {
                await session.SaveAsync(visibleIdentityResource.ToEntity());
                await session.SaveAsync(visibleApiResource.ToEntity());
                await session.SaveAsync(visibleApiScope.ToEntity());

                await session.SaveAsync(hiddenIdentityResource.ToEntity());
                await session.SaveAsync(hiddenApiResource.ToEntity());
                await session.SaveAsync(hiddenApiScope.ToEntity());

                await session.FlushAsync();
            }

            var loggerMock = new Mock<ILogger<ResourceStore>>();
            Resources resources;
            using (var session = testDb.OpenStatelessSession())
            {
                var store = new ResourceStore(session, loggerMock.Object);
                resources = await store.GetAllResourcesAsync();
            }

            resources.Should().NotBeNull();
            resources.IdentityResources.Should().NotBeEmpty();
            resources.IdentityResources.Count.Should().Be(2);
            resources.ApiResources.Should().NotBeEmpty();
            resources.ApiResources.Count.Should().Be(2);
            resources.ApiScopes.Should().NotBeEmpty();
            resources.ApiScopes.Count.Should().Be(2);

            resources.IdentityResources.Any(ir => ir.Name == visibleIdentityResource.Name).Should().BeTrue();
            resources.IdentityResources.Any(ir => ir.Name == hiddenIdentityResource.Name).Should().BeTrue();

            resources.ApiResources.Any(ar => ar.Name == visibleApiResource.Name).Should().BeTrue();
            resources.ApiResources.Any(ar => ar.Name == hiddenApiResource.Name).Should().BeTrue();

            resources.ApiScopes.Any(s => s.Name == visibleApiScope.Name).Should().BeTrue();
            resources.ApiScopes.Any(s => s.Name == hiddenApiScope.Name).Should().BeTrue();

            await CleanupTestDataAsync(testDb);
        }

        [Theory]
        [MemberData(nameof(TestDatabases))]
        public async Task Should_Retrieve_All_Resources_With_More_Than_One_Api_Scope(TestDatabase testDb)
        {
            var testIdentityResource = CreateTestIdentityResource("identity_resource_1");
            var testApiResource = CreateTestApiResource("test_api_resource_1", new[] {"user_claim1", "user_claim2"});
            var testApiScope1 = CreateTestApiScopeForResource("user_scope1", new[] { "user_scope1_claim1", "user_scope1_claim2" });
            var testApiScope2 = CreateTestApiScopeForResource("user_scope2", new[] { "user_scope2_claim1" });
            testApiResource.Scopes.Add(testApiScope1.Name);
            testApiResource.Scopes.Add(testApiScope2.Name);

            using (var session = testDb.OpenSession())
            {
                await session.SaveAsync(testIdentityResource.ToEntity());
                await session.SaveAsync(testApiResource.ToEntity());
                await session.SaveAsync(testApiScope1.ToEntity());
                await session.SaveAsync(testApiScope2.ToEntity());
                await session.FlushAsync();
            }

            var loggerMock = new Mock<ILogger<ResourceStore>>();
            Resources resources;
            using (var session = testDb.OpenStatelessSession())
            {
                var store = new ResourceStore(session, loggerMock.Object);
                resources = await store.GetAllResourcesAsync();
            }

            resources.Should().NotBeNull();
            resources.IdentityResources.Should().NotBeEmpty();
            resources.IdentityResources.Count.Should().Be(1);
            resources.ApiResources.Should().NotBeEmpty();
            resources.ApiResources.Count.Should().Be(1);
            resources.ApiResources.First().Scopes.Count.Should().Be(2);

            await CleanupTestDataAsync(testDb);
        }

        [Theory]
        [MemberData(nameof(TestDatabases))]
        public async Task Should_Retrieve_Identity_Resource_And_Its_Claims_By_Scope(TestDatabase testDb)
        {
            const string testIdentityResourceName = "idres1";
            var testIdentityResource = CreateTestIdentityResource(testIdentityResourceName);

            using (var session = testDb.OpenSession())
            {
                await session.SaveAsync(testIdentityResource.ToEntity());
                await session.FlushAsync();
            }

            var loggerMock = new Mock<ILogger<ResourceStore>>();
            IEnumerable<IdentityResource> resources;
            using (var session = testDb.OpenStatelessSession())
            {
                var store = new ResourceStore(session, loggerMock.Object);
                resources = (await store.FindIdentityResourcesByScopeNameAsync(new[] { testIdentityResourceName })).ToList();
            }

            resources.Should().NotBeEmpty();
            resources.Count().Should().Be(1);
            resources.First().Name.Should().Be(testIdentityResourceName);
            resources.First().UserClaims.Count.Should().Be(2);

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
            using (var session = testDb.OpenStatelessSession())
            {
                var store = new ResourceStore(session, loggerMock.Object);
                resources = (await store.FindIdentityResourcesByScopeNameAsync(new[] { testIdentityResource1.Name }))
                    .ToList();
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

        private static ApiResource CreateTestApiResource(string name, ICollection<string> userClaims)
        {
            var testApiResource = new ApiResource()
            {
                Name = name,
                ApiSecrets = new List<Secret> { new Secret("secret".ToSha256()) },
                UserClaims = userClaims
            };

            return testApiResource;
        }

        private static ApiScope CreateTestApiScopeForResource(string apiScopeName, ICollection<string> userClaims)
        {
            return new ApiScope()
            {
                Name = apiScopeName,
                UserClaims = userClaims
            };
        }

        private static async Task CleanupTestDataAsync(TestDatabase db)
        {
            using (var session = db.OpenSession())
            {
                using (var tx = session.BeginTransaction())
                {
                    await session.DeleteAsync("from IdentityResource ir");
                    await session.DeleteAsync("from ApiResource ar");
                    await session.DeleteAsync("from ApiScope s");
                    await tx.CommitAsync();
                }
            }
        }
    }
}
