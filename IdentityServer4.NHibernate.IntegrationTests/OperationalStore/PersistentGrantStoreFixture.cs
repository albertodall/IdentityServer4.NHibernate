using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using IdentityServer4.NHibernate.Extensions;
using IdentityServer4.NHibernate.IntegrationTests.TestStorage;
using IdentityServer4.NHibernate.Stores;
using IdentityServer4.Models;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace IdentityServer4.NHibernate.IntegrationTests.OperationalStore
{
    public class PersistentGrantStoreFixture : IntegrationTestFixture, IClassFixture<DatabaseFixture>
    {
        public static TheoryData<TestDatabase> TestDatabases;

        static PersistentGrantStoreFixture()
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

        public PersistentGrantStoreFixture(DatabaseFixture fixture)
        {
            var testDatabases = TestDatabases.SelectMany(t => t.Select(db => (TestDatabase)db)).ToList();
            fixture.TestDatabases = testDatabases;
        }

        [Theory]
        [MemberData(nameof(TestDatabases))]
        public async Task Should_Store_New_Grant(TestDatabase testDb)
        {
            var testGrant = CreateTestGrant();
            var loggerMock = new Mock<ILogger<PersistedGrantStore>>();

            using (var session = testDb.OpenSession())
            {
                var store = new PersistedGrantStore(session, loggerMock.Object);
                await store.StoreAsync(testGrant);
            }
           
            using (var session = testDb.OpenSession())
            {
                var foundGrant = await session.GetAsync<Entities.PersistedGrant>(testGrant.Key);
                foundGrant.Should().NotBeNull();
            }

            await CleanupTestDataAsync(testDb);
        }

        [Theory]
        [MemberData(nameof(TestDatabases))]
        public async Task Should_Get_Stored_Grant(TestDatabase testDb)
        {
            var testGrant = CreateTestGrant();
            var loggerMock = new Mock<ILogger<PersistedGrantStore>>();

            using (var session = testDb.OpenSession())
            {
                await session.SaveAsync(testGrant.ToEntity());
                await session.FlushAsync();
            }

            PersistedGrant foundGrant;
            using (var session = testDb.OpenSession())
            {
                var store = new PersistedGrantStore(session, loggerMock.Object);
                foundGrant = await store.GetAsync(testGrant.Key);
            }

            foundGrant.Should().NotBeNull();

            await CleanupTestDataAsync(testDb);
        }

        [Theory]
        [MemberData(nameof(TestDatabases))]
        public async Task Should_Retrieve_Grant_By_SubjectId(TestDatabase testDb)
        {
            var testGrant = CreateTestGrant();
            var loggerMock = new Mock<ILogger<PersistedGrantStore>>();

            using (var session = testDb.OpenSession())
            {
                await session.SaveAsync(testGrant.ToEntity());
                await session.FlushAsync();
            }

            IList<PersistedGrant> foundGrants;
            using (var session = testDb.OpenSession())
            {
                var store = new PersistedGrantStore(session, loggerMock.Object);
                foundGrants = (await store.GetAllAsync(testGrant.SubjectId)).ToList();
            }

            foundGrants.Should().NotBeEmpty();

            await CleanupTestDataAsync(testDb);
        }

        [Theory]
        [MemberData(nameof(TestDatabases))]
        public async Task Should_Remove_Persisted_Grant_By_Key(TestDatabase testDb)
        {
            var testGrant = CreateTestGrant();
            var loggerMock = new Mock<ILogger<PersistedGrantStore>>();

            using (var session = testDb.OpenSession())
            {
                await session.SaveAsync(testGrant.ToEntity());
                await session.FlushAsync();
            }

            using (var session = testDb.OpenSession())
            {
                var store = new PersistedGrantStore(session, loggerMock.Object);
                await store.RemoveAsync(testGrant.Key);
            }

            using (var session = testDb.OpenSession())
            {
                var foundGrant = await session.GetAsync<Entities.PersistedGrant>(testGrant.Key);
                foundGrant.Should().BeNull();
            }

            await CleanupTestDataAsync(testDb);
        }

        [Theory]
        [MemberData(nameof(TestDatabases))]
        public async Task Should_Remove_All_Persisted_Grant_By_Subject_And_Client(TestDatabase testDb)
        {
            var testGrant = CreateTestGrant();
            var loggerMock = new Mock<ILogger<PersistedGrantStore>>();

            using (var session = testDb.OpenSession())
            {
                await session.SaveAsync(testGrant.ToEntity());
                await session.FlushAsync();
            }

            using (var session = testDb.OpenSession())
            {
                var store = new PersistedGrantStore(session, loggerMock.Object);
                await store.RemoveAllAsync(testGrant.SubjectId, testGrant.ClientId);
            }

            using (var session = testDb.OpenSession())
            {
                var foundGrant = await session.GetAsync<Entities.PersistedGrant>(testGrant.Key);
                foundGrant.Should().BeNull();
            }

            await CleanupTestDataAsync(testDb);
        }

        [Theory]
        [MemberData(nameof(TestDatabases))]
        public async Task Should_Remove_All_Persisted_Grant_By_Subject_Client_Type(TestDatabase testDb)
        {
            var testGrant = CreateTestGrant();
            var loggerMock = new Mock<ILogger<PersistedGrantStore>>();

            using (var session = testDb.OpenSession())
            {
                await session.SaveAsync(testGrant.ToEntity());
                await session.FlushAsync();
            }

            using (var session = testDb.OpenSession())
            {
                var store = new PersistedGrantStore(session, loggerMock.Object);
                await store.RemoveAllAsync(testGrant.SubjectId, testGrant.ClientId, testGrant.Type);
            }

            using (var session = testDb.OpenSession())
            {
                var foundGrant = await session.GetAsync<Entities.PersistedGrant>(testGrant.Key);
                foundGrant.Should().BeNull();
            }

            await CleanupTestDataAsync(testDb);
        }

        [Theory]
        [MemberData(nameof(TestDatabases))]
        public async Task Should_Create_New_Grant_If_Key_Does_Not_Exist(TestDatabase testDb)
        {
            var testGrant = CreateTestGrant();
            var loggerMock = new Mock<ILogger<PersistedGrantStore>>();

            using (var session = testDb.OpenSession())
            {
                (await session.GetAsync<Entities.PersistedGrant>(testGrant.Key)).Should().BeNull();
            }

            using (var session = testDb.OpenSession())
            {
                var store = new PersistedGrantStore(session, loggerMock.Object);
                await store.StoreAsync(testGrant);
            }

            using (var session = testDb.OpenSession())
            {
                (await session.GetAsync<Entities.PersistedGrant>(testGrant.Key)).Should().NotBeNull();
            }

            await CleanupTestDataAsync(testDb);
        }

        [Theory]
        [MemberData(nameof(TestDatabases))]
        public async Task Should_Update_Grant_If_Key_Exists(TestDatabase testDb)
        {
            var testGrant = CreateTestGrant();
            var loggerMock = new Mock<ILogger<PersistedGrantStore>>();

            using (var session = testDb.OpenSession())
            {
                await session.SaveAsync(testGrant.ToEntity());
                await session.FlushAsync();
            }

            var newExpirationDate = testGrant.Expiration.Value.AddHours(1);
            using (var session = testDb.OpenSession())
            {
                var store = new PersistedGrantStore(session, loggerMock.Object);
                testGrant.Expiration = newExpirationDate;
                await store.StoreAsync(testGrant);
            }

            using (var session = testDb.OpenSession())
            {
                var foundGrant = await session.GetAsync<Entities.PersistedGrant>(testGrant.Key);
                foundGrant.Expiration.Value.Should().Be(newExpirationDate);
            }

            await CleanupTestDataAsync(testDb);
        }

        private static PersistedGrant CreateTestGrant()
        {
            return new PersistedGrant
            {
                Key = Guid.NewGuid().ToString(),
                Type = "authorization_code",
                ClientId = "test_client",
                SubjectId = "test_subject",
                CreationTime = new DateTime(2018, 11, 01),
                Expiration = new DateTime(2018, 11, 30),
                Data = "test data for this grant"
            };
        }

        private static async Task CleanupTestDataAsync(TestDatabase db)
        {
            using (var session = db.OpenSession())
            {
                using (var tx = session.BeginTransaction())
                {
                    await session.DeleteAsync("from PersistedGrant pg");
                    await tx.CommitAsync();
                }
            }
        }
    }
}
