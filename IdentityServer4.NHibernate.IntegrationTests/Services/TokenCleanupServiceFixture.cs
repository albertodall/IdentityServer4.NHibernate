using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.NHibernate.Entities;
using IdentityServer4.NHibernate.Extensions;
using IdentityServer4.NHibernate.IntegrationTests.TestStorage;
using IdentityServer4.NHibernate.Options;
using IdentityServer4.NHibernate.Stores;
using IdentityServer4.Stores;
using IdentityServer4.Test;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace IdentityServer4.NHibernate.IntegrationTests.Services
{
    public class TokenCleanupServiceFixture : IntegrationTestFixture, IClassFixture<DatabaseFixture>
    {
        public static TheoryData<TestDatabase> TestDatabases;

        static TokenCleanupServiceFixture()
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

        public TokenCleanupServiceFixture(DatabaseFixture fixture)
        {
            var testDatabases = TestDatabases.SelectMany(t => t.Select(db => (TestDatabase)db)).ToList();
            fixture.TestDatabases = testDatabases;
        }

        [Theory]
        [MemberData(nameof(TestDatabases))]
        public async Task Should_Remove_Expired_Grant(TestDatabase testDb)
        {
            var expiredGrant = new Models.PersistedGrant
            {
                Key = Guid.NewGuid().ToString(),
                ClientId = "test-app",
                Type = "reference",
                SubjectId = "42",
                Expiration = DateTime.UtcNow.AddDays(-3),
                Data = "testdata"
            };

            using (var session = testDb.OpenSession())
            {
                await session.SaveAsync(expiredGrant.ToEntity());
                await session.FlushAsync();
            }

            await CreateTokenCleanupServiceSut(testDb, TestOperationalStoreOptions).RemoveExpiredGrantsAsync();

            using (var session = testDb.OpenSession())
            {
                (await session.GetAsync<PersistedGrant>(expiredGrant.Key)).Should().BeNull();
            }

            await CleanupTestDataAsync(testDb);
        }

        [Theory]
        [MemberData(nameof(TestDatabases))]
        public async Task Should_Not_Remove_Still_Valid_Grant(TestDatabase testDb)
        {
            var validGrant = new Models.PersistedGrant
            {
                Key = Guid.NewGuid().ToString(),
                ClientId = "test-app",
                Type = "reference",
                SubjectId = "42",
                Expiration = DateTime.UtcNow.AddDays(3),
                Data = "testdata"
            };

            using (var session = testDb.OpenSession())
            {
                await session.SaveAsync(validGrant.ToEntity());
                await session.FlushAsync();
            }

            await CreateTokenCleanupServiceSut(testDb, TestOperationalStoreOptions).RemoveExpiredGrantsAsync();

            using (var session = testDb.OpenSession())
            {
                (await session.GetAsync<PersistedGrant>(validGrant.Key)).Should().NotBeNull();
            }

            await CleanupTestDataAsync(testDb);
        }

        [Theory]
        [MemberData(nameof(TestDatabases))]
        public async Task Should_Remove_Expired_Device_Code(TestDatabase testDb)
        {
            var expiredDeviceCode = new DeviceFlowCodes
            {
                DeviceCode = Guid.NewGuid().ToString(),
                ID = "1234",
                ClientId = "test-app",
                SubjectId = "42",
                CreationTime = DateTime.UtcNow.AddDays(-4),
                Expiration = DateTime.UtcNow.AddDays(-3),
                Data = "testdata"
            };

            using (var session = testDb.OpenSession())
            {
                await session.SaveAsync(expiredDeviceCode);
                await session.FlushAsync();
            }

            await CreateTokenCleanupServiceSut(testDb, TestOperationalStoreOptions).RemoveExpiredGrantsAsync();

            using (var session = testDb.OpenSession())
            {
                (await session.GetAsync<DeviceFlowCodes>(expiredDeviceCode.DeviceCode)).Should().BeNull();
            }

            await CleanupTestDataAsync(testDb);
        }

        [Theory]
        [MemberData(nameof(TestDatabases))]
        public async Task Should_Not_Remove_Still_Valid_Device_Code(TestDatabase testDb)
        {
            var validDeviceCode = new DeviceFlowCodes
            {
                DeviceCode = Guid.NewGuid().ToString(),
                ID = "1234",
                ClientId = "test-app",
                SubjectId = "42",
                CreationTime = DateTime.UtcNow.AddDays(-4),
                Expiration = DateTime.UtcNow.AddDays(3),
                Data = "testdata"
            };

            using (var session = testDb.OpenSession())
            {
                await session.SaveAsync(validDeviceCode);
                await session.FlushAsync();
            }

            await CreateTokenCleanupServiceSut(testDb, TestOperationalStoreOptions).RemoveExpiredGrantsAsync();

            using (var session = testDb.OpenSession())
            {
                (await session.GetAsync<DeviceFlowCodes>(validDeviceCode.ID)).Should().NotBeNull();
            }

            await CleanupTestDataAsync(testDb);
        }

        [Theory]
        [MemberData(nameof(TestDatabases))]
        public async Task Should_Remove_Expired_Grants_In_Batches(TestDatabase testDb)
        {
            using (var session = testDb.OpenSession())
            {
                for (var i = 0; i < 5; i++)
                {
                    await session.SaveAsync(new Models.PersistedGrant
                    {
                        Key = Guid.NewGuid().ToString(),
                        ClientId = "test-app",
                        Type = "reference",
                        SubjectId = (42 + i).ToString(),
                        Expiration = DateTime.UtcNow.AddDays(-3),
                        Data = $"testdata {i}"
                    }.ToEntity());
                }
                await session.FlushAsync();
            }

            var options = new OperationalStoreOptions()
            {
                TokenCleanupBatchSize = 2
            };
            await CreateTokenCleanupServiceSut(testDb, options).RemoveExpiredGrantsAsync();

            using (var session = testDb.OpenSession())
            {
                (await session.QueryOver<PersistedGrant>().RowCountAsync()).Should().Be(0);
            }

            await CleanupTestDataAsync(testDb);
        }

        [Theory]
        [MemberData(nameof(TestDatabases))]
        public async Task Should_Remove_Expired_Grants_In_Batches_Skipping_Still_Valid_Grants(TestDatabase testDb)
        {
            using (var session = testDb.OpenSession())
            {
                for (var i = 0; i < 5; i++)
                {
                    await session.SaveAsync(new Models.PersistedGrant
                    {
                        Key = Guid.NewGuid().ToString(),
                        ClientId = "test-app",
                        Type = "reference",
                        SubjectId = (42 + i).ToString(),
                        Expiration = DateTime.UtcNow.AddDays(-3),
                        Data = $"testdata {i}"
                    }.ToEntity());
                }
                await session.SaveAsync(new Models.PersistedGrant
                {
                    Key = Guid.NewGuid().ToString(),
                    ClientId = "test-app",
                    Type = "reference",
                    SubjectId = "99",
                    Expiration = DateTime.UtcNow.AddDays(3),
                    Data = "testdata 99"
                }.ToEntity());
                await session.FlushAsync();
            }

            var options = new OperationalStoreOptions()
            {
                TokenCleanupBatchSize = 3
            };
            await CreateTokenCleanupServiceSut(testDb, options).RemoveExpiredGrantsAsync();

            using (var session = testDb.OpenSession())
            {
                (await session.QueryOver<PersistedGrant>().RowCountAsync()).Should().Be(1);
            }

            await CleanupTestDataAsync(testDb);
        }

        [Theory]
        [MemberData(nameof(TestDatabases))]
        public async Task Should_Remove_Expired_Device_Codes_In_Batches(TestDatabase testDb)
        {
            using (var session = testDb.OpenSession())
            {
                for (var i = 0; i < 5; i++)
                {
                    await session.SaveAsync(new DeviceFlowCodes
                    {
                        DeviceCode = Guid.NewGuid().ToString(),
                        ID = $"1234{i}",
                        ClientId = "test-app",
                        SubjectId = (42 + i).ToString(),
                        CreationTime = DateTime.UtcNow.AddDays(-4),
                        Expiration = DateTime.UtcNow.AddDays(-3),
                        Data = $"testdata {i}"
                    });
                }
                await session.FlushAsync();
            }

            var options = new OperationalStoreOptions()
            {
                TokenCleanupBatchSize = 2
            };
            await CreateTokenCleanupServiceSut(testDb, options).RemoveExpiredGrantsAsync();

            using (var session = testDb.OpenSession())
            {
                (await session.QueryOver<DeviceFlowCodes>().RowCountAsync()).Should().Be(0);
            }

            await CleanupTestDataAsync(testDb);
        }

        [Theory]
        [MemberData(nameof(TestDatabases))]
        public async Task Should_Remove_Expired_Device_Codes_In_Batches_Keeping_Still_Valid_Codes(TestDatabase testDb)
        {
            using (var session = testDb.OpenSession())
            {
                for (var i = 0; i < 5; i++)
                {
                    await session.SaveAsync(new DeviceFlowCodes
                    {
                        DeviceCode = Guid.NewGuid().ToString(),
                        ID = $"1234{i}",
                        ClientId = "test-app",
                        SubjectId = (42 + i).ToString(),
                        CreationTime = DateTime.UtcNow.AddDays(-4),
                        Expiration = DateTime.UtcNow.AddDays(-3),
                        Data = $"testdata {i}"
                    });
                }
                await session.SaveAsync(new DeviceFlowCodes
                {
                    DeviceCode = Guid.NewGuid().ToString(),
                    ID = "123499",
                    ClientId = "test-app",
                    SubjectId = "99",
                    CreationTime = DateTime.UtcNow.AddDays(-4),
                    Expiration = DateTime.UtcNow.AddDays(3),
                    Data = "testdata 99"
                });
                await session.FlushAsync();
            }

            var options = new OperationalStoreOptions()
            {
                TokenCleanupBatchSize = 2
            };
            await CreateTokenCleanupServiceSut(testDb, options).RemoveExpiredGrantsAsync();

            using (var session = testDb.OpenSession())
            {
                (await session.QueryOver<DeviceFlowCodes>().RowCountAsync()).Should().Be(1);
            }

            await CleanupTestDataAsync(testDb);
        }

        private static TokenCleanupService CreateTokenCleanupServiceSut(TestDatabase testDb, OperationalStoreOptions options)
        {
            IServiceCollection services = new ServiceCollection();
            services.AddIdentityServer()
                .AddTestUsers(new List<TestUser>())
                .AddInMemoryClients(new List<Models.Client>())
                .AddInMemoryIdentityResources(new List<Models.IdentityResource>())
                .AddInMemoryApiResources(new List<Models.ApiResource>());

            services.AddSingleton(testDb.SessionFactory);
            services.AddScoped(provider => testDb.OpenStatelessSession());

            services.AddTransient<IPersistedGrantStore, PersistedGrantStore>();
            services.AddTransient<IDeviceFlowStore, DeviceFlowStore>();
            services.AddSingleton(options);
            services.AddTransient<TokenCleanupService>();

            return services.BuildServiceProvider().GetRequiredService<TokenCleanupService>();
        }

        private static async Task CleanupTestDataAsync(TestDatabase db)
        {
            using (var session = db.OpenSession())
            {
                using (var tx = session.BeginTransaction())
                {
                    await session.DeleteAsync("from PersistedGrant pg");
                    await session.DeleteAsync("from DeviceFlowCodes c");
                    await tx.CommitAsync();
                }
            }
        }
    }
}
