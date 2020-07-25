using System;
using System.Linq;
using System.Reflection;
using IdentityServer4.Models;
using IdentityServer4.NHibernate.Extensions;
using IdentityServer4.NHibernate.Services;
using IdentityServer4.NHibernate.IntegrationTests.TestStorage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using FluentAssertions;
using Moq;
using Xunit;

namespace IdentityServer4.NHibernate.IntegrationTests.Services
{
    public class CorsPolicyServiceFixture : IntegrationTestFixture, IClassFixture<DatabaseFixture>
    {
        public static TheoryData<TestDatabase> TestDatabases;

        static CorsPolicyServiceFixture()
        {
            var sqlServerDataSource = TestSettings["SQLServer"];

            TestDatabases = new TheoryData<TestDatabase>()
            {
                TestDatabaseBuilder.SQLServer2012TestDatabase(sqlServerDataSource, $"{MethodBase.GetCurrentMethod()?.DeclaringType?.Name}_NH_Test", TestConfigurationStoreOptions, TestOperationalStoreOptions),
                TestDatabaseBuilder.SQLiteTestDatabase($"{MethodBase.GetCurrentMethod()?.DeclaringType?.Name}_NH_Test.sqlite", TestConfigurationStoreOptions, TestOperationalStoreOptions),
                TestDatabaseBuilder.SQLiteInMemoryTestDatabase(TestConfigurationStoreOptions, TestOperationalStoreOptions)
            };
        }

        public CorsPolicyServiceFixture(DatabaseFixture fixture)
        {
            var testDatabases = TestDatabases.SelectMany(t => t.Select(db => (TestDatabase)db)).ToList();
            fixture.TestDatabases = testDatabases;
        }

        [Theory]
        [MemberData(nameof(TestDatabases))]
        public void Should_Check_For_Allowed_Origin(TestDatabase testDb)
        {
            const string testCorsOrigin = "https://www.site1.it/";

            using (var session = testDb.SessionFactory.OpenSession())
            {
                session.Save(
                    new Client()
                    {
                        ClientId = Guid.NewGuid().ToString(),
                        ClientName = "1st_client",
                        AllowedCorsOrigins = new[] { "https://www.site2.com" }
                    }.ToEntity()
                );
                
                session.Save(
                    new Client
                    {
                        ClientId = Guid.NewGuid().ToString(),
                        ClientName = "2nd_client",
                        AllowedCorsOrigins = new[] { "https://www.site2.com", testCorsOrigin }
                    }.ToEntity()
                );
                session.Flush();
            }

            var ctx = new DefaultHttpContext();
            var svcs = new ServiceCollection();
            svcs.AddScoped(provider => testDb.OpenStatelessSession());
            ctx.RequestServices = svcs.BuildServiceProvider();

            var ctxAccessor = new HttpContextAccessor()
            {
                HttpContext = ctx
            };

            var loggerMock = new Mock<ILogger<CorsPolicyService>>();
            var service = new CorsPolicyService(ctxAccessor, loggerMock.Object);
            var result = service.IsOriginAllowedAsync(testCorsOrigin).Result;

            result.Should().BeTrue();

            CleanupTestData(testDb);
        }

        [Theory]
        [MemberData(nameof(TestDatabases))]
        public void Should_Check_For_Not_Allowed_Origin(TestDatabase testDb)
        {
            using (var session = testDb.OpenSession())
            {
                session.Save(
                    new Client()
                    {
                        ClientId = Guid.NewGuid().ToString(),
                        ClientName = "test_client",
                        AllowedCorsOrigins = new[] { "https://allowed.site.it" }
                    }.ToEntity()
                );
                session.Flush();
            }

            var ctx = new DefaultHttpContext();
            var svcs = new ServiceCollection();
            svcs.AddScoped(provider => testDb.OpenStatelessSession());
            ctx.RequestServices = svcs.BuildServiceProvider();

            var ctxAccessor = new HttpContextAccessor()
            {
                HttpContext = ctx
            };

            var loggerMock = new Mock<ILogger<CorsPolicyService>>();
            var service = new CorsPolicyService(ctxAccessor, loggerMock.Object);
            var result = service.IsOriginAllowedAsync("https://not.allowed.site.it").Result;

            result.Should().BeFalse();

            CleanupTestData(testDb);
        }

        private static void CleanupTestData(TestDatabase db)
        {
            using (var session = db.OpenSession())
            {
                using (var tx = session.BeginTransaction())
                {
                    session.Delete("from Client c");
                    tx.Commit();
                }
            }
        }
    }
}
