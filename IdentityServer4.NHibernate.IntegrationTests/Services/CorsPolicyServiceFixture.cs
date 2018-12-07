using System.Linq;
using IdentityServer4.Models;
using IdentityServer4.NHibernate.Extensions;
using IdentityServer4.NHibernate.Options;
using IdentityServer4.NHibernate.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using FluentAssertions;
using Moq;
using Xunit;

namespace IdentityServer4.NHibernate.IntegrationTests.Services
{
    public class CorsPolicyServiceFixture : IClassFixture<DatabaseFixture>
    {
        private static readonly ConfigurationStoreOptions ConfigurationStoreOptions = new ConfigurationStoreOptions();
        private static readonly OperationalStoreOptions OperationalStoreOptions = new OperationalStoreOptions();

        public static readonly TheoryData<TestDatabase> TestDatabases = new TheoryData<TestDatabase>()
        {
            TestDatabaseBuilder.SQLServer2012TestDatabase("(local)", "CorsPolicyService_NH_Test", ConfigurationStoreOptions, OperationalStoreOptions)
        };

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
                        ClientId = "1st_client",
                        ClientName = "1st_client",
                        AllowedCorsOrigins = new[] { "https://www.site2.com" }
                    }.ToEntity()
                );
                
                session.Save(
                    new Client
                    {
                        ClientId = "2nd_client",
                        ClientName = "2nd_client",
                        AllowedCorsOrigins = new[] { "https://www.site2.com", testCorsOrigin }
                    }.ToEntity()
                );
                session.Flush();
            }

            bool result;
            var ctx = new DefaultHttpContext();
            var svcs = new ServiceCollection();
            svcs.AddScoped(provider =>
            {
                return testDb.SessionFactory.OpenStatelessSession();
            });
            ctx.RequestServices = svcs.BuildServiceProvider();

            var ctxAccessor = new HttpContextAccessor()
            {
                HttpContext = ctx
            };

            var loggerMock = new Mock<ILogger<CorsPolicyService>>();
            var service = new CorsPolicyService(ctxAccessor, loggerMock.Object);
            result = service.IsOriginAllowedAsync(testCorsOrigin).Result;

            result.Should().BeTrue();
        }

        [Theory]
        [MemberData(nameof(TestDatabases))]
        public void Should_Check_For_Not_Allowed_Origin(TestDatabase testDb)
        {
            using (var session = testDb.SessionFactory.OpenSession())
            {
                session.Save(
                    new Client()
                    {
                        ClientId = "test_client",
                        ClientName = "test_client",
                        AllowedCorsOrigins = new[] { "https://allowed.site.it" }
                    }.ToEntity()
                );
                session.Flush();
            }

            bool result;
            var ctx = new DefaultHttpContext();
            var svcs = new ServiceCollection();
            svcs.AddScoped(provider =>
            {
                return testDb.SessionFactory.OpenStatelessSession();
            });
            ctx.RequestServices = svcs.BuildServiceProvider();

            var ctxAccessor = new HttpContextAccessor()
            {
                HttpContext = ctx
            };

            var loggerMock = new Mock<ILogger<CorsPolicyService>>();
            var service = new CorsPolicyService(ctxAccessor, loggerMock.Object);
            result = service.IsOriginAllowedAsync("https://not.allowed.site.it").Result;

            result.Should().BeFalse();
        }
    }
}
