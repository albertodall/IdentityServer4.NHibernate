﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using IdentityServer4.NHibernate.Extensions;
using IdentityServer4.NHibernate.IntegrationTests.TestStorage;
using IdentityServer4.NHibernate.Stores;
using IdentityServer4.Models;
using FluentAssertions;
using IdentityServer4.Stores;
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
            var sqlServerDataSource = TestSettings["SQLServer"];

            TestDatabases = new TheoryData<TestDatabase>()
            {
                TestDatabaseBuilder.SQLServer2012TestDatabase(sqlServerDataSource, $"{MethodBase.GetCurrentMethod().DeclaringType.Name}_NH_Test", TestConfigurationStoreOptions, TestOperationalStoreOptions),
                TestDatabaseBuilder.SQLiteTestDatabase($"{MethodBase.GetCurrentMethod().DeclaringType.Name}_NH_Test.sqlite", TestConfigurationStoreOptions, TestOperationalStoreOptions),
                TestDatabaseBuilder.SQLiteInMemoryTestDatabase(TestConfigurationStoreOptions, TestOperationalStoreOptions)
            };
        }

        public PersistentGrantStoreFixture(DatabaseFixture fixture)
        {
            var testDatabases = TestDatabases.SelectMany(t => t.Select(db => (TestDatabase)db)).ToList();
            fixture.TestDatabases = testDatabases;
        }

        [Theory]
        [MemberData(nameof(TestDatabases))]
        public void Should_Store_New_Grant(TestDatabase testDb)
        {
            var testGrant = CreateTestGrant();
            var loggerMock = new Mock<ILogger<PersistedGrantStore>>();

            using (var session = testDb.OpenSession())
            {
                var store = new PersistedGrantStore(session, loggerMock.Object);
                store.StoreAsync(testGrant).Wait();
            }
           
            using (var session = testDb.OpenSession())
            {
                var foundGrant = session.Get<Entities.PersistedGrant>(testGrant.Key);
                foundGrant.Should().NotBeNull();
            }

            CleanupTestData(testDb);
        }

        [Theory]
        [MemberData(nameof(TestDatabases))]
        public void Should_Get_Stored_Grant(TestDatabase testDb)
        {
            var testGrant = CreateTestGrant();
            var loggerMock = new Mock<ILogger<PersistedGrantStore>>();

            using (var session = testDb.OpenSession())
            {
                session.Save(testGrant.ToEntity());
                session.Flush();
            }

            PersistedGrant foundGrant;
            using (var session = testDb.OpenSession())
            {
                var store = new PersistedGrantStore(session, loggerMock.Object);
                foundGrant = store.GetAsync(testGrant.Key).Result;
            }

            foundGrant.Should().NotBeNull();

            CleanupTestData(testDb);
        }

        [Theory]
        [MemberData(nameof(TestDatabases))]
        public void Should_Retrieve_Grant_By_SubjectId(TestDatabase testDb)
        {
            var testGrant = CreateTestGrant();
            var loggerMock = new Mock<ILogger<PersistedGrantStore>>();

            using (var session = testDb.OpenSession())
            {
                session.Save(testGrant.ToEntity());
                session.Flush();
            }

            IList<PersistedGrant> foundGrants;
            using (var session = testDb.OpenSession())
            {
                var store = new PersistedGrantStore(session, loggerMock.Object);
                foundGrants = store.GetAllAsync(testGrant.SubjectId).Result.ToList();
            }

            foundGrants.Should().NotBeEmpty();

            CleanupTestData(testDb);
        }

        [Theory]
        [MemberData(nameof(TestDatabases))]
        public void Should_Remove_Persisted_Grant_By_Key(TestDatabase testDb)
        {
            var testGrant = CreateTestGrant();
            var loggerMock = new Mock<ILogger<PersistedGrantStore>>();

            using (var session = testDb.OpenSession())
            {
                session.Save(testGrant.ToEntity());
                session.Flush();
            }

            using (var session = testDb.OpenSession())
            {
                var store = new PersistedGrantStore(session, loggerMock.Object);
                store.RemoveAsync(testGrant.Key).Wait();
            }

            using (var session = testDb.OpenSession())
            {
                var foundGrant = session.Get<Entities.PersistedGrant>(testGrant.Key);
                foundGrant.Should().BeNull();
            }

            CleanupTestData(testDb);
        }

        [Theory]
        [MemberData(nameof(TestDatabases))]
        public void Should_Remove_All_Persisted_Grant_By_Subject_And_Client(TestDatabase testDb)
        {
            var testGrant = CreateTestGrant();
            var loggerMock = new Mock<ILogger<PersistedGrantStore>>();

            using (var session = testDb.OpenSession())
            {
                session.Save(testGrant.ToEntity());
                session.Flush();
            }

            var filter = new PersistedGrantFilter()
            {
                SubjectId = testGrant.SubjectId,
                ClientId = testGrant.ClientId
            };

            using (var session = testDb.OpenSession())
            {
                var store = new PersistedGrantStore(session, loggerMock.Object);
                store.RemoveAllAsync(filter).Wait();
            }

            using (var session = testDb.OpenSession())
            {
                var foundGrant = session.Get<Entities.PersistedGrant>(testGrant.Key);
                foundGrant.Should().BeNull();
            }

            CleanupTestData(testDb);
        }

        [Theory]
        [MemberData(nameof(TestDatabases))]
        public void Should_Remove_All_Persisted_Grant_By_Subject_Client_Type(TestDatabase testDb)
        {
            var testGrant = CreateTestGrant();
            var loggerMock = new Mock<ILogger<PersistedGrantStore>>();

            using (var session = testDb.OpenSession())
            {
                session.Save(testGrant.ToEntity());
                session.Flush();
            }

            var filter = new PersistedGrantFilter()
            {
                SubjectId = testGrant.SubjectId,
                ClientId = testGrant.ClientId,
                Type = testGrant.Type
            };

            using (var session = testDb.OpenSession())
            {
                var store = new PersistedGrantStore(session, loggerMock.Object);
                store.RemoveAllAsync(filter).Wait();
            }

            using (var session = testDb.OpenSession())
            {
                var foundGrant = session.Get<Entities.PersistedGrant>(testGrant.Key);
                foundGrant.Should().BeNull();
            }

            CleanupTestData(testDb);
        }

        [Theory]
        [MemberData(nameof(TestDatabases))]
        public void Should_Create_New_Grant_If_Key_Does_Not_Exist(TestDatabase testDb)
        {
            var testGrant = CreateTestGrant();
            var loggerMock = new Mock<ILogger<PersistedGrantStore>>();

            using (var session = testDb.OpenSession())
            {
                session.Get<Entities.PersistedGrant>(testGrant.Key).Should().BeNull();
            }

            using (var session = testDb.OpenSession())
            {
                var store = new PersistedGrantStore(session, loggerMock.Object);
                store.StoreAsync(testGrant).Wait();
            }

            using (var session = testDb.OpenSession())
            {
                session.Get<Entities.PersistedGrant>(testGrant.Key).Should().NotBeNull();
            }

            CleanupTestData(testDb);
        }

        [Theory]
        [MemberData(nameof(TestDatabases))]
        public void Should_Update_Grant_If_Key_Exists(TestDatabase testDb)
        {
            var testGrant = CreateTestGrant();
            var loggerMock = new Mock<ILogger<PersistedGrantStore>>();

            using (var session = testDb.OpenSession())
            {
                session.Save(testGrant.ToEntity());
                session.Flush();
            }

            var newExpirationDate = testGrant.Expiration?.AddHours(1);
            using (var session = testDb.OpenSession())
            {
                var store = new PersistedGrantStore(session, loggerMock.Object);
                testGrant.Expiration = newExpirationDate;
                store.StoreAsync(testGrant).Wait();
            }

            using (var session = testDb.OpenSession())
            {
                var foundGrant = session.Get<Entities.PersistedGrant>(testGrant.Key);
                foundGrant.Expiration.Value.Should().Be(newExpirationDate.Value);
            }

            CleanupTestData(testDb);
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

        private static void CleanupTestData(TestDatabase db)
        {
            using (var session = db.OpenSession())
            {
                using (var tx = session.BeginTransaction())
                {
                    session.Delete("from PersistedGrant pg");
                    tx.Commit();
                }
            }
        }
    }
}
