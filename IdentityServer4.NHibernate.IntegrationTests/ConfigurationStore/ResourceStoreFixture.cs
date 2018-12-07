using System;
using System.Collections.Generic;
using System.Linq;
using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.NHibernate.Extensions;
using IdentityServer4.NHibernate.Options;
using IdentityServer4.NHibernate.Stores;
using Microsoft.Extensions.Logging;
using FluentAssertions;
using Moq;
using Xunit;

namespace IdentityServer4.NHibernate.IntegrationTests.ConfigurationStore
{
    public class ResourceStoreFixture : IClassFixture<DatabaseFixture>
    {
        private static readonly ConfigurationStoreOptions ConfigurationStoreOptions = new ConfigurationStoreOptions();
        private static readonly OperationalStoreOptions OperationalStoreOptions = new OperationalStoreOptions();

        public static readonly TheoryData<TestDatabase> TestDatabases = new TheoryData<TestDatabase>()
        {
            TestDatabaseBuilder.SQLServer2012TestDatabase("(local)", "ResourceStore_NH_Test", ConfigurationStoreOptions, OperationalStoreOptions)
        };

        public ResourceStoreFixture(DatabaseFixture fixture)
        {
            var testDatabases = TestDatabases.SelectMany(t => t.Select(db => (TestDatabase)db)).ToList();
            fixture.TestDatabases = testDatabases;
        }

        [Theory]
        [MemberData(nameof(TestDatabases))]
        public void Should_Retrieve_Existing_Api_Resources_By_Scope_Names(TestDatabase testDb)
        {
            string testScope1 = "ar1_scope1";
            string testScope2 = "ar1_scope2";

            var testApiResource1 = CreateTestApiResource("test_api_resource1", new[] { testScope1, testScope2 });

            using (var session = testDb.SessionFactory.OpenSession())
            {
                session.Save(testApiResource1.ToEntity());
                session.Flush();
            }

            var loggerMock = new Mock<ILogger<ResourceStore>>();
            IEnumerable<ApiResource> resources;
            using (var session = testDb.SessionFactory.OpenSession())
            {
                var store = new ResourceStore(session, loggerMock.Object);
                resources = store.FindApiResourcesByScopeAsync(new string[] { testScope1, testScope2 }).Result;
            }

            resources.Count().Should().Be(1);
            resources.FirstOrDefault(x => x.Name == testApiResource1.Name).Should().NotBeNull();
            resources.First().Scopes.Count().Should().Be(2);
            resources.First().Scopes.First().UserClaims.Count().Should().Be(1);
            resources.First().UserClaims.Count().Should().Be(2);
        }

        [Theory]
        [MemberData(nameof(TestDatabases))]
        public void Should_Retrieve_Requested_Api_Resources_By_Scope_Names(TestDatabase testDb)
        {
            string testScope1 = "ar2_scope1";
            string testScope2 = "ar2_scope2";

            var testApiResource1 = CreateTestApiResource("test_api_resource2", new[] { testScope1, testScope2 });
            var testApiResource2 = CreateTestApiResource("test_api_resource3", new[] { "ar3_scope3" });

            using (var session = testDb.SessionFactory.OpenSession())
            {
                session.Save(testApiResource1.ToEntity());
                session.Save(testApiResource2.ToEntity());
                session.Flush();
            }

            var loggerMock = new Mock<ILogger<ResourceStore>>();
            IEnumerable<ApiResource> resources;
            using (var session = testDb.SessionFactory.OpenSession())
            {
                var store = new ResourceStore(session, loggerMock.Object);
                resources = store.FindApiResourcesByScopeAsync(new string[] { testScope1, testScope2 }).Result;
            }

            resources.Count().Should().Be(1);
            resources.FirstOrDefault(x => x.Name == testApiResource1.Name).Should().NotBeNull();
            resources.First().Scopes.Count().Should().Be(2);
            resources.First().Scopes.First().UserClaims.Count().Should().Be(1);
            resources.First().UserClaims.Count().Should().Be(2);
        }

        [Theory]
        [MemberData(nameof(TestDatabases))]
        public void Should_Retrieve_All_Api_Resources_By_Scope_Names(TestDatabase testDb)
        {
            string testScope1 = Guid.NewGuid().ToString();
            string testScope2 = Guid.NewGuid().ToString();
            string testScope3 = Guid.NewGuid().ToString();

            var testApiResource1 = CreateTestApiResource(Guid.NewGuid().ToString(), new[] { testScope1, testScope2 });
            var testApiResource2 = CreateTestApiResource(Guid.NewGuid().ToString(), new[] { testScope3 });

            using (var session = testDb.SessionFactory.OpenSession())
            {
                session.Save(testApiResource1.ToEntity());
                session.Save(testApiResource2.ToEntity());
                session.Flush();
            }

            var loggerMock = new Mock<ILogger<ResourceStore>>();
            IEnumerable<ApiResource> resources;
            using (var session = testDb.SessionFactory.OpenSession())
            {
                var store = new ResourceStore(session, loggerMock.Object);
                resources = store.FindApiResourcesByScopeAsync(new string[] { testScope1, testScope2, testScope3 }).Result;
            }

            resources.Count().Should().Be(2);
        }

        [Theory]
        [MemberData(nameof(TestDatabases))]
        public void Should_Not_Retrieve_Api_Resources_With_Unexisting_Scope_Name(TestDatabase testDb)
        {
            string testScope1 = "test_api_resource_scope1";
            string testScope2 = "test_api_resource_scope2";

            var testApiResource = CreateTestApiResource("test_api_resource", new[] { testScope1, testScope2 });

            using (var session = testDb.SessionFactory.OpenSession())
            {
                session.Save(testApiResource.ToEntity());
                session.Flush();
            }

            var loggerMock = new Mock<ILogger<ResourceStore>>();
            IEnumerable<ApiResource> resources;
            using (var session = testDb.SessionFactory.OpenSession())
            {
                var store = new ResourceStore(session, loggerMock.Object);
                resources = store.FindApiResourcesByScopeAsync(new string[] { "non_existing_scope" }).Result;
            }

            resources.Should().BeEmpty();
        }

        [Theory]
        [MemberData(nameof(TestDatabases))]
        public void Should_Retrieve_Existing_Api_Resource_By_Name(TestDatabase testDb)
        {
            string testScope1 = "ar4_scope1";
            var testApiResource = CreateTestApiResource("test_api_resource4", new[] { testScope1 });

            using (var session = testDb.SessionFactory.OpenSession())
            {
                session.Save(testApiResource.ToEntity());
                session.Flush();
            }

            var loggerMock = new Mock<ILogger<ResourceStore>>();
            ApiResource resource;
            using (var session = testDb.SessionFactory.OpenSession())
            {
                var store = new ResourceStore(session, loggerMock.Object);
                resource = store.FindApiResourceAsync(testApiResource.Name).Result;
            }

            resource.Should().NotBeNull();
            resource.ApiSecrets.Should().NotBeEmpty();
            resource.Scopes.Should().NotBeEmpty();
            resource.Scopes.Count().Should().Be(1);
            resource.Scopes.First().UserClaims.Should().NotBeEmpty();
            resource.Scopes.First().UserClaims.Count().Should().Be(1);
            resource.UserClaims.Count().Should().Be(2);
        }

        [Theory]
        [MemberData(nameof(TestDatabases))]
        public void Should_Not_Retrieve_Non_Existing_Api_Resource(TestDatabase testDb)
        {
            var loggerMock = new Mock<ILogger<ResourceStore>>();
            ApiResource resource;
            using (var session = testDb.SessionFactory.OpenSession())
            {
                var store = new ResourceStore(session, loggerMock.Object);
                resource = store.FindApiResourceAsync("non_existing_api_resource").Result;
            }

            resource.Should().BeNull();
        }

        [Theory]
        [MemberData(nameof(TestDatabases))]
        public void Should_Retrieve_All_Resources_Included_Hidden_Ones(TestDatabase testDb)
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

            using (var session = testDb.SessionFactory.OpenSession())
            {
                session.Save(visibleIdentityResource.ToEntity());
                session.Save(visibleApiResource.ToEntity());
                session.Save(hiddenIdentityResource.ToEntity());
                session.Save(hiddenApiResource.ToEntity());
                session.Flush();
            }

            var loggerMock = new Mock<ILogger<ResourceStore>>();
            Resources resources;
            using (var session = testDb.SessionFactory.OpenSession())
            {
                var store = new ResourceStore(session, loggerMock.Object);
                resources = store.GetAllResourcesAsync().Result;
            }

            resources.Should().NotBeNull();
            resources.IdentityResources.Should().NotBeEmpty();
            resources.ApiResources.Should().NotBeEmpty();

            Assert.Contains(resources.IdentityResources, x => !x.ShowInDiscoveryDocument);
            Assert.Contains(resources.ApiResources, x => !x.Scopes.Any(y => y.ShowInDiscoveryDocument));
        }

        [Theory]
        [MemberData(nameof(TestDatabases))]
        public void Should_Retrieve_Identity_Resource_And_Its_Claims_By_Scope(TestDatabase testDb)
        {
            var testIdentityResourceName = "idres1";
            var testIdentityResource = CreateTestIdentityResource(testIdentityResourceName);

            using (var session = testDb.SessionFactory.OpenSession())
            {
                session.Save(testIdentityResource.ToEntity());
                session.Flush();
            }

            var loggerMock = new Mock<ILogger<ResourceStore>>();
            IEnumerable<IdentityResource> resources;
            using (var session = testDb.SessionFactory.OpenSession())
            {
                var store = new ResourceStore(session, loggerMock.Object);
                resources = store.FindIdentityResourcesByScopeAsync(
                    new[] { testIdentityResourceName})
                    .Result;
            }

            resources.Should().NotBeEmpty();
            resources.First().Name.Should().Be(testIdentityResourceName);
            resources.First().UserClaims.Count().Should().Be(2);
        }

        [Theory]
        [MemberData(nameof(TestDatabases))]
        public void Should_Retrieve_Only_Requested_Identity_Resources(TestDatabase testDb)
        {
            var testIdentityResource1 = CreateTestIdentityResource(Guid.NewGuid().ToString());
            var testIdentityResource2 = CreateTestIdentityResource(Guid.NewGuid().ToString());

            using (var session = testDb.SessionFactory.OpenSession())
            {
                session.Save(testIdentityResource1.ToEntity());
                session.Save(testIdentityResource2.ToEntity());
                session.Flush();
            }

            var loggerMock = new Mock<ILogger<ResourceStore>>();
            IEnumerable<IdentityResource> resources;
            using (var session = testDb.SessionFactory.OpenSession())
            {
                var store = new ResourceStore(session, loggerMock.Object);
                resources = store.FindIdentityResourcesByScopeAsync(
                    new[] { testIdentityResource1.Name })
                    .Result;
            }

            resources.Should().NotBeEmpty();
            resources.Count().Should().Be(1);
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
    }
}