using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.Models;
using IdentityServer4.NHibernate.Extensions;
using IdentityServer4.NHibernate.IntegrationTests.TestStorage;
using IdentityServer4.NHibernate.Options;
using IdentityServer4.NHibernate.Stores;
using IdentityServer4.Stores;
using IdentityServer4.Test;
using Microsoft.Extensions.DependencyInjection;
using NHibernate;
using Xunit;

namespace IdentityServer4.NHibernate.IntegrationTests.Services
{
    public class TokenCleanupServiceFixture : IntegrationTestFixture, IClassFixture<DatabaseFixture>
    {
        public static TheoryData<TestDatabase> TestDatabases;

        static TokenCleanupServiceFixture()
        {
            var sqlServerDataSource = TestSettings["SQLServer"];

            TestDatabases = new TheoryData<TestDatabase>()
            {
                TestDatabaseBuilder.SQLServer2012TestDatabase(sqlServerDataSource, $"{MethodBase.GetCurrentMethod()?.DeclaringType?.Name}_NH_Test", TestConfigurationStoreOptions, TestOperationalStoreOptions),
                TestDatabaseBuilder.SQLiteTestDatabase($"{MethodBase.GetCurrentMethod()?.DeclaringType?.Name}_NH_Test.sqlite", TestConfigurationStoreOptions, TestOperationalStoreOptions),
                TestDatabaseBuilder.SQLiteInMemoryTestDatabase(TestConfigurationStoreOptions, TestOperationalStoreOptions)
            };
        }

        public TokenCleanupServiceFixture(DatabaseFixture fixture)
        {
            var testDatabases = TestDatabases.SelectMany(t => t.Select(db => (TestDatabase)db)).ToList();
            fixture.TestDatabases = testDatabases;
        }

        [Theory]
        [MemberData(nameof(TestDatabases))]
        public async Task Should_Remove_Expired_Grant(TestDatabase testDb)
        {
            var expiredGrant = new PersistedGrant
            {
                Key = Guid.NewGuid().ToString(),
                ClientId = "app1",
                Type = "reference",
                SubjectId = "123",
                Expiration = DateTime.UtcNow.AddDays(-3),
                Data = "{!}"
            };

            using (var session = testDb.OpenSession())
            {
                await session.SaveAsync(expiredGrant.ToEntity());
                await session.FlushAsync();
            }

            await CreateSut(testDb).RemoveExpiredGrantsAsync();

            using (var session = testDb.OpenSession())
            {
                (await session.GetAsync<Entities.PersistedGrant>(expiredGrant.Key)).Should().BeNull();
            }
        }

        private static TokenCleanupService CreateSut(TestDatabase testDb)
        {
            IServiceCollection services = new ServiceCollection();
            services.AddIdentityServer()
                .AddTestUsers(new List<TestUser>())
                .AddInMemoryClients(new List<Models.Client>())
                .AddInMemoryIdentityResources(new List<IdentityResource>())
                .AddInMemoryApiResources(new List<ApiResource>());

            services.AddSingleton(testDb.SessionFactory);
            services.AddScoped(provider => testDb.OpenSession());
            services.AddScoped(provider => testDb.OpenStatelessSession());

            services.AddTransient<IPersistedGrantStore, PersistedGrantStore>();
            services.AddTransient<IDeviceFlowStore, DeviceFlowStore>();
            services.AddSingleton(new OperationalStoreOptions());
            services.AddTransient<TokenCleanupService>();

            return services.BuildServiceProvider().GetRequiredService<TokenCleanupService>();
        }
    }
}
