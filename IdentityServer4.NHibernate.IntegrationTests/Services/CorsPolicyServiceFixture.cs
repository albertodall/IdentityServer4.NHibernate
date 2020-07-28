using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.NHibernate.Extensions;
using IdentityServer4.NHibernate.IntegrationTests.TestStorage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using FluentAssertions;
using IdentityServer4.NHibernate.Services;
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
        public async Task Should_Check_For_Allowed_Origin(TestDatabase testDb)
        {
            const string testCorsOrigin = "https://www.site1.it/";

            using (var session = testDb.SessionFactory.OpenSession())
            {
                await session.SaveAsync(
                    new Client()
                    {
                        ClientId = Guid.NewGuid().ToString(),
                        ClientName = "1st_client",
                        AllowedCorsOrigins = new[] { "https://www.site2.com" }
                    }.ToEntity()
                );
                
                await session.SaveAsync(
                    new Client
                    {
                        ClientId = Guid.NewGuid().ToString(),
                        ClientName = "2nd_client",
                        AllowedCorsOrigins = new[] { "https://www.site2.com", testCorsOrigin }
                    }.ToEntity()
                );
                await session.FlushAsync();
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
            var result = await service.IsOriginAllowedAsync(testCorsOrigin);

            result.Should().BeTrue();

            await CleanupTestDataAsync(testDb);
        }

        [Theory]
        [MemberData(nameof(TestDatabases))]
        public async Task Should_Check_For_Not_Allowed_Origin(TestDatabase testDb)
        {
            using (var session = testDb.OpenSession())
            {
                await session.SaveAsync(
                    new Client()
                    {
                        ClientId = Guid.NewGuid().ToString(),
                        ClientName = "test_client",
                        AllowedCorsOrigins = new[] { "https://allowed.site.it" }
                    }.ToEntity()
                );
                await session.FlushAsync();
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
            var result = await service.IsOriginAllowedAsync("https://not.allowed.site.it");

            result.Should().BeFalse();

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
