namespace IdentityServer4.NHibernate.IntegrationTests.ConfigurationStore
{
    using System.Collections.Generic;
    using System.Linq;
    using Extensions;
    using Options;
    using IdentityModel;
    using IdentityServer4.Models;
    using Xunit;
    using IdentityServer4.NHibernate.Stores;
    using Microsoft.Extensions.Logging;
    using Moq;
    using FluentAssertions;

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
            var testIdentityResource = CreateTestIdentityResource();
            var testApiResource = CreateTestApiResource();

            using (var session = testDb.SessionFactory.OpenSession())
            {
                using (var tx = session.BeginTransaction())
                {
                    session.Save(testIdentityResource.ToEntity());
                    session.Save(testApiResource.ToEntity());
                    tx.Commit();
                }
            }

            var loggerMock = new Mock<ILogger<ResourceStore>>();
            IEnumerable<ApiResource> resources;
            using (var session = testDb.SessionFactory.OpenSession())
            {
                var store = new ResourceStore(session, loggerMock.Object);
                resources = store.FindApiResourcesByScopeAsync(new string[] { testIdentityResource.Name, testApiResource.Scopes.First().Name }).Result;
            }

            resources.Should().NotBeNull();
        }

        private static IdentityResource CreateTestIdentityResource()
        {
            return new IdentityResource()
            {
                Name = "test_identity_resource",
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

        private static ApiResource CreateTestApiResource()
        {
            return new ApiResource()
            {
                Name = "test_api_resource",
                ApiSecrets = new List<Secret> { new Secret("secret".Sha256()) },
                Scopes =
                    new List<Scope>
                    {
                        new Scope()
                        {
                            Name = "test_scope",
                            UserClaims = { "test_claim" } 
                        }
                    },
                UserClaims =
                {
                    "user_claim_1",
                    "user_claim_2"
                }
            };
        }
    }
}
