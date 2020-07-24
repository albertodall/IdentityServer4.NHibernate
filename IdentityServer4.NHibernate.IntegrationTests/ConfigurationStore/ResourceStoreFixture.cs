using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
                TestDatabaseBuilder.SQLServer2012TestDatabase(sqlServerDataSource, $"{MethodBase.GetCurrentMethod().DeclaringType.Name}_NH_Test", TestConfigurationStoreOptions, TestOperationalStoreOptions),
                TestDatabaseBuilder.SQLiteTestDatabase($"{MethodBase.GetCurrentMethod().DeclaringType.Name}_NH_Test.sqlite", TestConfigurationStoreOptions, TestOperationalStoreOptions),
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
        public void Should_Retrieve_Existing_Api_Resources_By_Scope_Names(TestDatabase testDb)
        {
            const string userClaim1 = "ar1_userclaim1";
            const string userClaim2 = "ar1_userclaim2";

            var testApiResource1 = CreateTestApiResource("test_api_resource1", new [] { userClaim1, userClaim2 });
            var testApiScope1 = CreateTestApiScopeForResource("test_api_scope1", new[] {"scope1", "scope2"});
            testApiResource1.Scopes.Add(testApiScope1.Name);

            using (var session = testDb.OpenSession())
            {
                session.Save(testApiResource1.ToEntity());
                session.Save(testApiScope1.ToEntity());
                session.Flush();
            }

            var loggerMock = new Mock<ILogger<ResourceStore>>();
            IEnumerable<ApiResource> resources;
            using (var session = testDb.OpenSession())
            {
                var store = new ResourceStore(session, loggerMock.Object);
                resources = store.FindApiResourcesByScopeNameAsync(new[] { testApiScope1.Name }).Result.ToList();
            }

            resources.Count().Should().Be(1);
            resources.First().Scopes.Count.Should().Be(1);
            resources.First().UserClaims.Count.Should().Be(2);

            CleanupTestData(testDb);
        }

        [Theory]
        [MemberData(nameof(TestDatabases))]
        public void Should_Retrieve_Requested_Api_Resources_By_Scope_Names(TestDatabase testDb)
        {
            const string testScope1 = "ar2_scope1";
            const string testScope2 = "ar2_scope2";
            const string testScope3 = "ar2_scope3";

            var testApiResource1 = CreateTestApiResource("test_api_resource2", new[] { testScope1, testScope2 });
            var testApiScope1 = CreateTestApiScopeForResource("scope_r1", new[] {"u_scope3", "u_scope4"});
            testApiResource1.Scopes.Add(testApiScope1.Name);
            var testApiResource2 = CreateTestApiResource("test_api_resource3", new[] { testScope3 });
            var testApiScope2 = CreateTestApiScopeForResource("scope_r2", new[] { "u_scope5", "u_scope6" });
            testApiResource2.Scopes.Add(testApiScope2.Name);

            using (var session = testDb.OpenSession())
            {
                session.Save(testApiResource1.ToEntity());
                session.Save(testApiResource2.ToEntity());
                session.Save(testApiScope1.ToEntity());
                session.Save(testApiScope2.ToEntity());
                session.Flush();
            }

            var loggerMock = new Mock<ILogger<ResourceStore>>();
            IEnumerable<ApiResource> resources;
            using (var session = testDb.OpenSession())
            {
                var store = new ResourceStore(session, loggerMock.Object);
                resources = store.FindApiResourcesByScopeNameAsync(new[] { testScope1, testScope2 }).Result.ToList();
            }

            resources.Count().Should().Be(1);
            resources.First().Scopes.Count.Should().Be(2);
            resources.First().UserClaims.Count.Should().Be(2);

            CleanupTestData(testDb);
        }

        [Theory]
        [MemberData(nameof(TestDatabases))]
        public void Should_Retrieve_All_Api_Resources_By_Scope_Names(TestDatabase testDb)
        {
            var testScope1 = Guid.NewGuid().ToString();
            var testScope2 = Guid.NewGuid().ToString();
            var testScope3 = Guid.NewGuid().ToString();

            var testApiResource1 = CreateTestApiResource(Guid.NewGuid().ToString(), new[] { testScope1, testScope2 });
            var testApiResource2 = CreateTestApiResource(Guid.NewGuid().ToString(), new[] { testScope3 });

            using (var session = testDb.OpenSession())
            {
                session.Save(testApiResource1.ToEntity());
                session.Save(testApiResource2.ToEntity());
                session.Flush();
            }

            var loggerMock = new Mock<ILogger<ResourceStore>>();
            IEnumerable<ApiResource> resources;
            using (var session = testDb.OpenSession())
            {
                var store = new ResourceStore(session, loggerMock.Object);
                resources = store.FindApiResourcesByScopeNameAsync(new[] { testScope1, testScope2, testScope3 }).Result;
            }

            resources.Count().Should().Be(2);

            CleanupTestData(testDb);
        }

        [Theory]
        [MemberData(nameof(TestDatabases))]
        public void Should_Not_Retrieve_Api_Resources_With_Unexisting_Scope_Name(TestDatabase testDb)
        {
            const string testScope1 = "test_api_resource_scope1";
            const string testScope2 = "test_api_resource_scope2";

            var testApiResource = CreateTestApiResource("test_api_resource", new[] { testScope1, testScope2 });

            using (var session = testDb.OpenSession())
            {
                session.Save(testApiResource.ToEntity());
                session.Flush();
            }

            var loggerMock = new Mock<ILogger<ResourceStore>>();
            IEnumerable<ApiResource> resources;
            using (var session = testDb.OpenSession())
            {
                var store = new ResourceStore(session, loggerMock.Object);
                resources = store.FindApiResourcesByScopeNameAsync(new[] { "non_existing_scope" }).Result;
            }

            resources.Should().BeEmpty();

            CleanupTestData(testDb);
        }

        [Theory]
        [MemberData(nameof(TestDatabases))]
        public void Should_Retrieve_Existing_Api_Resource_By_Name(TestDatabase testDb)
        {
            const string testScope1 = "ar4_scope1";
            var testApiResource = CreateTestApiResource("test_api_resource4", new[] { testScope1 });

            using (var session = testDb.OpenSession())
            {
                session.Save(testApiResource.ToEntity());
                session.Flush();
            }

            var loggerMock = new Mock<ILogger<ResourceStore>>();
            ApiResource resource;
            using (var session = testDb.OpenSession())
            {
                var store = new ResourceStore(session, loggerMock.Object);
                resource = store.FindApiResourcesByNameAsync(new[] { testApiResource.Name }).Result.Single();
            }

            resource.Should().NotBeNull();
            resource.ApiSecrets.Should().NotBeEmpty();
            resource.Scopes.Should().NotBeEmpty();
            resource.Scopes.Count.Should().Be(1);
            resource.Scopes.Should().NotBeEmpty();
            resource.Scopes.Count.Should().Be(1);
            resource.UserClaims.Count.Should().Be(2);

            CleanupTestData(testDb);
        }

        [Theory]
        [MemberData(nameof(TestDatabases))]
        public void Should_Not_Retrieve_Non_Existing_Api_Resource(TestDatabase testDb)
        {
            var loggerMock = new Mock<ILogger<ResourceStore>>();
            ApiResource resource;
            using (var session = testDb.OpenSession())
            {
                var store = new ResourceStore(session, loggerMock.Object);
                resource = store.FindApiResourcesByNameAsync(new [] { "non_existing_api_resource" }).Result.SingleOrDefault();
            }

            resource.Should().BeNull();
        }

        //[Theory]
        //[MemberData(nameof(TestDatabases))]
        //public void Should_Retrieve_All_Resources_Included_Hidden_Ones(TestDatabase testDb)
        //{
        //    var visibleIdentityResource = CreateTestIdentityResource("identity_visible");
        //    var visibleApiResource = CreateTestApiResource("api_visible", new[] { "api_visible_scope1", "api_visible_scope_2" });
        //    var hiddenIdentityResource = new IdentityResource()
        //    {
        //        Name = "identity_hidden",
        //        ShowInDiscoveryDocument = false
        //    };
        //    var hiddenApiResource = new ApiResource()
        //    {
        //        Name = "api_hidden",
        //        Scopes = { "scope_hidden_1" }
        //    };

        //    using (var session = testDb.OpenSession())
        //    {
        //        session.Save(visibleIdentityResource.ToEntity());
        //        session.Save(visibleApiResource.ToEntity());
        //        session.Save(hiddenIdentityResource.ToEntity());
        //        session.Save(hiddenApiResource.ToEntity());
        //        session.Flush();
        //    }

        //    var loggerMock = new Mock<ILogger<ResourceStore>>();
        //    Resources resources;
        //    using (var session = testDb.OpenSession())
        //    {
        //        var store = new ResourceStore(session, loggerMock.Object);
        //        resources = store.GetAllResourcesAsync().Result;
        //    }

        //    resources.Should().NotBeNull();
        //    resources.IdentityResources.Should().NotBeEmpty();
        //    resources.ApiResources.Should().NotBeEmpty();

        //    Assert.Contains(resources.IdentityResources, x => !x.ShowInDiscoveryDocument);
        //    Assert.Contains(resources.ApiResources, x => !x.Scopes.Any(y => y.ShowInDiscoveryDocument));

        //    CleanupTestData(testDb);
        //}

        [Theory]
        [MemberData(nameof(TestDatabases))]
        public void Should_Retrieve_All_Resources_With_More_Than_One_Api_Scope(TestDatabase testDb)
        {
            var testIdentityResource = CreateTestIdentityResource("identity_resource_1");
            var testApiResource = CreateTestApiResource("test_api_resource_1", new[] {"user_claim_1", "user_claim_2"});
            var testApiScope = CreateTestApiScopeForResource("user_scope",
                new[] {"test_ar_1_user_claim_1", "test_ar_1_user_claim_2"});
            testApiResource.Scopes.Add(testApiScope.Name);

            using (var session = testDb.OpenSession())
            {
                session.Save(testIdentityResource.ToEntity());
                session.Save(testApiResource.ToEntity());
                session.Save(testApiScope);
                session.Flush();
            }

            var loggerMock = new Mock<ILogger<ResourceStore>>();
            Resources resources;
            using (var session = testDb.OpenSession())
            {
                var store = new ResourceStore(session, loggerMock.Object);
                resources = store.GetAllResourcesAsync().Result;
            }

            resources.Should().NotBeNull();
            resources.IdentityResources.Should().NotBeEmpty();
            resources.ApiResources.Count.Should().Be(1);
            resources.ApiResources.First().Scopes.Count().Should().Be(1);

            CleanupTestData(testDb);
        }

        [Theory]
        [MemberData(nameof(TestDatabases))]
        public void Should_Retrieve_Identity_Resource_And_Its_Claims_By_Scope(TestDatabase testDb)
        {
            var testIdentityResourceName = "idres1";
            var testIdentityResource = CreateTestIdentityResource(testIdentityResourceName);

            using (var session = testDb.OpenSession())
            {
                session.Save(testIdentityResource.ToEntity());
                session.Flush();
            }

            var loggerMock = new Mock<ILogger<ResourceStore>>();
            IEnumerable<IdentityResource> resources;
            using (var session = testDb.OpenSession())
            {
                var store = new ResourceStore(session, loggerMock.Object);
                resources = store.FindIdentityResourcesByScopeNameAsync(new[] { testIdentityResourceName})
                    .Result
                    .ToList();
            }

            resources.Should().NotBeEmpty();
            resources.First().Name.Should().Be(testIdentityResourceName);
            resources.First().UserClaims.Count.Should().Be(2);

            CleanupTestData(testDb);
        }

        [Theory]
        [MemberData(nameof(TestDatabases))]
        public void Should_Retrieve_Only_Requested_Identity_Resources(TestDatabase testDb)
        {
            var testIdentityResource1 = CreateTestIdentityResource(Guid.NewGuid().ToString());
            var testIdentityResource2 = CreateTestIdentityResource(Guid.NewGuid().ToString());

            using (var session = testDb.OpenSession())
            {
                session.Save(testIdentityResource1.ToEntity());
                session.Save(testIdentityResource2.ToEntity());
                session.Flush();
            }

            var loggerMock = new Mock<ILogger<ResourceStore>>();
            IEnumerable<IdentityResource> resources;
            using (var session = testDb.OpenSession())
            {
                var store = new ResourceStore(session, loggerMock.Object);
                resources = store.FindIdentityResourcesByScopeNameAsync(new[] { testIdentityResource1.Name })
                    .Result
                    .ToList();
            }

            resources.Should().NotBeEmpty();
            resources.Count().Should().Be(1);

            CleanupTestData(testDb);
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
                Scopes = new[] { Guid.NewGuid().ToString() },
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

        private static void CleanupTestData(TestDatabase db)
        {
            using (var session = db.OpenSession())
            {
                using (var tx = session.BeginTransaction())
                {
                    session.Delete("from IdentityResource ir");
                    session.Delete("from ApiResource ar");
                    tx.Commit();
                }
            }
        }
    }
}