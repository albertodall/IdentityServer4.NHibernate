namespace IdentityServer4.NHibernate.IntegrationTests.ConfigurationStore
{
    using System;
    using System.Linq;
    using Extensions;
    using Options;
    using Stores;
    using IdentityServer4.Models;
    using FluentAssertions;
    using Moq;
    using Microsoft.Extensions.Logging;
    using Xunit;
    using System.Collections.Generic;

    public class PersistentGrantStoreFixture : IClassFixture<DatabaseFixture>
    {
        private static readonly ConfigurationStoreOptions ConfigurationStoreOptions = new ConfigurationStoreOptions();
        private static readonly OperationalStoreOptions OperationalStoreOptions = new OperationalStoreOptions();

        public static readonly TheoryData<TestDatabase> TestDatabases = new TheoryData<TestDatabase>()
        {
            TestDatabaseBuilder.SQLServer2012TestDatabase("(local)", "PersistentGrantStore_NH_Test", ConfigurationStoreOptions, OperationalStoreOptions)
        };

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

            using (var session = testDb.SessionFactory.OpenSession())
            {
                var store = new PersistedGrantStore(session, loggerMock.Object);
                store.StoreAsync(testGrant).Wait();
            }
           
            using (var session = testDb.SessionFactory.OpenSession())
            {
                var foundGrant = session.Get<Entities.PersistedGrant>(testGrant.Key);
                foundGrant.Should().NotBeNull();
            }
        }

        [Theory]
        [MemberData(nameof(TestDatabases))]
        public void Should_Get_Stored_Grant(TestDatabase testDb)
        {
            var testGrant = CreateTestGrant();
            var loggerMock = new Mock<ILogger<PersistedGrantStore>>();

            using (var session = testDb.SessionFactory.OpenSession())
            {
                session.Save(testGrant.ToEntity());
                session.Flush();
            }

            PersistedGrant foundGrant;
            using (var session = testDb.SessionFactory.OpenSession())
            {
                var store = new PersistedGrantStore(session, loggerMock.Object);
                foundGrant = store.GetAsync(testGrant.Key).Result;
            }

            foundGrant.Should().NotBeNull();
        }

        [Theory]
        [MemberData(nameof(TestDatabases))]
        public void Should_Retrieve_Grant_By_SubjectId(TestDatabase testDb)
        {
            var testGrant = CreateTestGrant();
            var loggerMock = new Mock<ILogger<PersistedGrantStore>>();

            using (var session = testDb.SessionFactory.OpenSession())
            {
                session.Save(testGrant.ToEntity());
                session.Flush();
            }

            IList<PersistedGrant> foundGrants;
            using (var session = testDb.SessionFactory.OpenSession())
            {
                var store = new PersistedGrantStore(session, loggerMock.Object);
                foundGrants = store.GetAllAsync(testGrant.SubjectId).Result.ToList();
            }

            foundGrants.Should().NotBeEmpty();
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
    }
}
