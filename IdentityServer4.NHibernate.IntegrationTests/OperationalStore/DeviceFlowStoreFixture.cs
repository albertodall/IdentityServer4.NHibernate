using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.NHibernate.Entities;
using IdentityServer4.NHibernate.Options;
using IdentityServer4.NHibernate.Stores;
using IdentityServer4.Stores.Serialization;
using Microsoft.Extensions.Logging;
using Moq;
using NHibernate;
using Xunit;

namespace IdentityServer4.NHibernate.IntegrationTests.OperationalStore
{
    public class DeviceFlowStoreFixture : IClassFixture<DatabaseFixture>
    {
        private static readonly ConfigurationStoreOptions ConfigurationStoreOptions = new ConfigurationStoreOptions();
        private static readonly OperationalStoreOptions OperationalStoreOptions = new OperationalStoreOptions();

        public static readonly TheoryData<TestDatabase> TestDatabases = new TheoryData<TestDatabase>()
        {
            TestDatabaseBuilder.SQLServer2012TestDatabase("(local)", "DeviceFlowStore_NH_Test", ConfigurationStoreOptions, OperationalStoreOptions)
        };

        public DeviceFlowStoreFixture(DatabaseFixture fixture)
        {
            var testDatabases = TestDatabases.SelectMany(t => t.Select(db => (TestDatabase)db)).ToList();
            fixture.TestDatabases = testDatabases;
        }

        [Theory]
        [MemberData(nameof(TestDatabases))]
        public async Task Should_Store_DeviceCode_And_UserCode_When_Authorization_Successful(TestDatabase testDb)
        {
            var loggerMock = new Mock<ILogger<DeviceFlowStore>>();

            var deviceCode = Guid.NewGuid().ToString();
            var userCode = Guid.NewGuid().ToString();
            var data = new DeviceCode
            {
                ClientId = Guid.NewGuid().ToString(),
                CreationTime = DateTime.UtcNow,
                Lifetime = 42
            };

            using (var session = testDb.SessionFactory.OpenSession())
            {
                var store = new DeviceFlowStore(session, new PersistentGrantSerializer(), loggerMock.Object);
                await store.StoreDeviceAuthorizationAsync(deviceCode, userCode, data);
            }

            using (var session = testDb.SessionFactory.OpenSession())
            {
                var foundDeviceFlowCodes = await session.QueryOver<DeviceFlowCodes>()
                    .Where(c => c.DeviceCode == deviceCode)
                    .SingleOrDefaultAsync();

                foundDeviceFlowCodes.Should().NotBeNull();
                foundDeviceFlowCodes?.DeviceCode.Should().Be(deviceCode);
                foundDeviceFlowCodes?.ID.Should().Be(userCode);
            }
        }

        [Theory]
        [MemberData(nameof(TestDatabases))]
        public async Task Should_Find_Stored_Data(TestDatabase testDb)
        {
            var loggerMock = new Mock<ILogger<DeviceFlowStore>>();
            var serializer = new PersistentGrantSerializer();

            var deviceCode = Guid.NewGuid().ToString();
            var userCode = Guid.NewGuid().ToString();
            var data = new DeviceCode()
            {
                ClientId = Guid.NewGuid().ToString(),
                CreationTime = DateTime.UtcNow,
                Lifetime = 300
            };

            using (var session = testDb.SessionFactory.OpenSession())
            {
                var store = new DeviceFlowStore(session, new PersistentGrantSerializer(), loggerMock.Object);
                await store.StoreDeviceAuthorizationAsync(deviceCode, userCode, data);
            }

            using (var session = testDb.SessionFactory.OpenSession())
            {
                var foundDeviceFlowCodes = await session.QueryOver<DeviceFlowCodes>()
                    .Where(c => c.DeviceCode == deviceCode)
                    .SingleOrDefaultAsync();

                foundDeviceFlowCodes.Should().NotBeNull();
                var deserializedData = serializer.Deserialize<DeviceCode>(foundDeviceFlowCodes?.Data);

                deserializedData.CreationTime.Should().BeCloseTo(data.CreationTime);
                deserializedData.ClientId.Should().Be(data.ClientId);
                deserializedData.Lifetime.Should().Be(data.Lifetime);
            }
        }

        [Theory]
        [MemberData(nameof(TestDatabases))]
        public async Task Should_Throw_Exception_If_DeviceCode_Already_Exists(TestDatabase testDb)
        {
            var loggerMock = new Mock<ILogger<DeviceFlowStore>>();
            var serializer = new PersistentGrantSerializer();

            var existingUserCode = $"user_{Guid.NewGuid().ToString()}";
            var deviceCodeData = new DeviceCode()
            {
                ClientId = "device_flow",
                RequestedScopes = new[] { "openid", "api1" },
                CreationTime = new DateTime(2018, 12, 07, 8, 00, 00),
                Lifetime = 300,
                IsOpenId = true,
                Subject = new ClaimsPrincipal(
                    new ClaimsIdentity(
                        new List<Claim> { new Claim(JwtClaimTypes.Subject, $"sub_{Guid.NewGuid().ToString()}") }
                    )
                )
            };

            using (var session = testDb.SessionFactory.OpenSession())
            {
                using (var tx = session.BeginTransaction())
                {
                    await session.SaveAsync(new DeviceFlowCodes()
                    {
                        DeviceCode = $"device_{Guid.NewGuid().ToString()}",
                        ID = existingUserCode,
                        ClientId = deviceCodeData.ClientId,
                        SubjectId = deviceCodeData.Subject.FindFirst(JwtClaimTypes.Subject).Value,
                        CreationTime = deviceCodeData.CreationTime,
                        Expiration = deviceCodeData.CreationTime.AddSeconds(deviceCodeData.Lifetime),
                        Data = serializer.Serialize(deviceCodeData)
                    });
                    await tx.CommitAsync();
                }
            }

            using (var session = testDb.SessionFactory.OpenSession())
            {
                var store = new DeviceFlowStore(session, new PersistentGrantSerializer(), loggerMock.Object);

                Func<Task> act = async () => await store.StoreDeviceAuthorizationAsync($"device_{Guid.NewGuid().ToString()}", existingUserCode, deviceCodeData);

                await act.Should().ThrowAsync<HibernateException>();
            }
        }

        [Theory]
        [MemberData(nameof(TestDatabases))]
        public async Task Should_Find_Existing_UserCode(TestDatabase testDb)
        {
            var loggerMock = new Mock<ILogger<DeviceFlowStore>>();
            var serializer = new PersistentGrantSerializer();

            var testDeviceCode = $"device_{Guid.NewGuid().ToString()}";
            var testUserCode = $"user_{Guid.NewGuid().ToString()}";

            var expectedSubject = $"sub_{Guid.NewGuid().ToString()}";
            var expectedDeviceCodeData = new DeviceCode()
            {
                ClientId = "device_flow",
                RequestedScopes = new[] { "openid", "api1" },
                CreationTime = new DateTime(2018, 12, 07, 14, 15, 16),
                Lifetime = 42,
                IsOpenId = true,
                Subject = new ClaimsPrincipal(
                    new ClaimsIdentity(
                        new List<Claim> { new Claim(JwtClaimTypes.Subject, expectedSubject) }
                    )
                )
            };

            using (var session = testDb.SessionFactory.OpenSession())
            {
                using (var tx = session.BeginTransaction())
                {
                    await session.SaveAsync(new DeviceFlowCodes()
                    {
                        DeviceCode = testDeviceCode,
                        ID = testUserCode,
                        ClientId = expectedDeviceCodeData.ClientId,
                        SubjectId = expectedDeviceCodeData.Subject.FindFirst(JwtClaimTypes.Subject).Value,
                        CreationTime = expectedDeviceCodeData.CreationTime,
                        Expiration = expectedDeviceCodeData.CreationTime.AddSeconds(expectedDeviceCodeData.Lifetime),
                        Data = serializer.Serialize(expectedDeviceCodeData)
                    });
                    await tx.CommitAsync();
                }
            }

            DeviceCode code;
            using (var session = testDb.SessionFactory.OpenSession())
            {
                var store = new DeviceFlowStore(session, serializer, loggerMock.Object);
                code = await store.FindByUserCodeAsync(testUserCode);
            }

            code.Should().BeEquivalentTo(expectedDeviceCodeData, assertionOptions => assertionOptions.Excluding(x => x.Subject));
            code.Subject.Claims.FirstOrDefault(x => x.Type == JwtClaimTypes.Subject && x.Value == expectedSubject).Should().NotBeNull();
        }

        [Theory]
        [MemberData(nameof(TestDatabases))]
        public async Task Should_Return_Null_If_UserCode_Does_Not_Exist(TestDatabase testDb)
        {
            var loggerMock = new Mock<ILogger<DeviceFlowStore>>();

            using (var session = testDb.SessionFactory.OpenSession())
            {
                var store = new DeviceFlowStore(session, new PersistentGrantSerializer(), loggerMock.Object);
                var code = await store.FindByUserCodeAsync($"user_{Guid.NewGuid().ToString()}");
                code.Should().BeNull();
            }
        }

        [Theory]
        [MemberData(nameof(TestDatabases))]
        public async Task Should_Retrieve_Data_By_DeviceCode(TestDatabase testDb)
        {
            var loggerMock = new Mock<ILogger<DeviceFlowStore>>();
            var serializer = new PersistentGrantSerializer();

            var testDeviceCode = $"device_{Guid.NewGuid().ToString()}";
            var testUserCode = $"user_{Guid.NewGuid().ToString()}";

            var expectedSubject = $"sub_{Guid.NewGuid().ToString()}";
            var expectedDeviceCodeData = new DeviceCode()
            {
                ClientId = "device_flow",
                RequestedScopes = new[] { "openid", "api1" },
                CreationTime = new DateTime(2018, 12, 7, 14, 15, 16),
                Lifetime = 300,
                IsOpenId = true,
                Subject = new ClaimsPrincipal(
                    new ClaimsIdentity(
                        new List<Claim> { new Claim(JwtClaimTypes.Subject, expectedSubject) }
                    )
                )
            };

            using (var session = testDb.SessionFactory.OpenSession())
            {
                using (var tx = session.BeginTransaction())
                {
                    await session.SaveAsync(new DeviceFlowCodes()
                    {
                        DeviceCode = testDeviceCode,
                        ID = testUserCode,
                        ClientId = expectedDeviceCodeData.ClientId,
                        SubjectId = expectedDeviceCodeData.Subject.FindFirst(JwtClaimTypes.Subject).Value,
                        CreationTime = expectedDeviceCodeData.CreationTime,
                        Expiration = expectedDeviceCodeData.CreationTime.AddSeconds(expectedDeviceCodeData.Lifetime),
                        Data = serializer.Serialize(expectedDeviceCodeData)
                    });
                    await tx.CommitAsync();
                }
            }

            DeviceCode code;
            using (var session = testDb.SessionFactory.OpenSession())
            {
                var store = new DeviceFlowStore(session, serializer, loggerMock.Object);
                code = await store.FindByDeviceCodeAsync(testDeviceCode);
            }

            code.Should().BeEquivalentTo(expectedDeviceCodeData, assertionOptions => assertionOptions.Excluding(x => x.Subject));
            code.Subject.Claims.FirstOrDefault(x => x.Type == JwtClaimTypes.Subject && x.Value == expectedSubject).Should().NotBeNull();
        }

        [Theory]
        [MemberData(nameof(TestDatabases))]
        public async Task Should_Retrieve_Null_If_DeviceCode_Does_Not_Exist(TestDatabase testDb)
        {
            var loggerMock = new Mock<ILogger<DeviceFlowStore>>();
            using (var session = testDb.SessionFactory.OpenSession())
            {
                var store = new DeviceFlowStore(session, new PersistentGrantSerializer(), loggerMock.Object);
                var code = await store.FindByDeviceCodeAsync($"device_{Guid.NewGuid().ToString()}");
                code.Should().BeNull();
            }
        }

        [Theory]
        [MemberData(nameof(TestDatabases))]
        public async Task Should_Update_Authorized_Device_Code(TestDatabase testDb)
        {
            var loggerMock = new Mock<ILogger<DeviceFlowStore>>();
            var serializer = new PersistentGrantSerializer();

            var testDeviceCode = $"device_{Guid.NewGuid().ToString()}";
            var testUserCode = $"user_{Guid.NewGuid().ToString()}";

            var expectedSubject = $"sub_{Guid.NewGuid().ToString()}";
            var unauthorizedDeviceCode = new DeviceCode()
            {
                ClientId = "device_flow",
                RequestedScopes = new[] { "openid", "api1" },
                CreationTime = new DateTime(2018, 12, 7, 14, 15, 16),
                Lifetime = 42,
                IsOpenId = true
            };

            using (var session = testDb.SessionFactory.OpenSession())
            {
                using (var tx = session.BeginTransaction())
                {
                    await session.SaveAsync(new DeviceFlowCodes
                    {
                        DeviceCode = testDeviceCode,
                        ID = testUserCode,
                        ClientId = unauthorizedDeviceCode.ClientId,
                        CreationTime = unauthorizedDeviceCode.CreationTime,
                        Expiration = unauthorizedDeviceCode.CreationTime.AddSeconds(unauthorizedDeviceCode.Lifetime),
                        Data = serializer.Serialize(unauthorizedDeviceCode)
                    });
                    await tx.CommitAsync();
                }
            }

            var authorizedDeviceCode = new DeviceCode()
            {
                ClientId = unauthorizedDeviceCode.ClientId,
                RequestedScopes = unauthorizedDeviceCode.RequestedScopes,
                AuthorizedScopes = unauthorizedDeviceCode.RequestedScopes,
                Subject = new ClaimsPrincipal(
                    new ClaimsIdentity(
                        new List<Claim> { new Claim(JwtClaimTypes.Subject, expectedSubject) }
                     )
                ),
                IsAuthorized = true,
                IsOpenId = true,
                CreationTime = new DateTime(2018, 12, 7, 14, 15, 16),
                Lifetime = 42
            };

            using (var session = testDb.SessionFactory.OpenSession())
            {
                var store = new DeviceFlowStore(session, serializer, loggerMock.Object);
                await store.UpdateByUserCodeAsync(testUserCode, authorizedDeviceCode);
            }

            DeviceFlowCodes updatedCodes;
            using (var session = testDb.SessionFactory.OpenSession())
            {
                updatedCodes = await session.GetAsync<DeviceFlowCodes>(testUserCode);
            }

            // Unchanged
            updatedCodes.DeviceCode.Should().Be(testDeviceCode);
            updatedCodes.ClientId.Should().Be(unauthorizedDeviceCode.ClientId);
            updatedCodes.CreationTime.Should().Be(unauthorizedDeviceCode.CreationTime);
            updatedCodes.Expiration.Should().Be(unauthorizedDeviceCode.CreationTime.AddSeconds(unauthorizedDeviceCode.Lifetime));

            // Updated
            var parsedCode = serializer.Deserialize<DeviceCode>(updatedCodes.Data);
            parsedCode.Should().BeEquivalentTo(authorizedDeviceCode, assertionOptions => assertionOptions.Excluding(x => x.Subject));
            parsedCode.Subject.Claims.FirstOrDefault(x => x.Type == JwtClaimTypes.Subject && x.Value == expectedSubject).Should().NotBeNull();
        }

        [Theory]
        [MemberData(nameof(TestDatabases))]
        public async Task Should_Remove_Existing_DeviceCode(TestDatabase testDb)
        {
            var loggerMock = new Mock<ILogger<DeviceFlowStore>>();
            var serializer = new PersistentGrantSerializer();

            var testDeviceCode = $"device_{Guid.NewGuid().ToString()}";
            var testUserCode = $"user_{Guid.NewGuid().ToString()}";

            var existingDeviceCode = new DeviceCode
            {
                ClientId = "device_flow",
                RequestedScopes = new[] { "openid", "api1" },
                CreationTime = new DateTime(2018, 12, 7, 14, 15, 16),
                Lifetime = 42,
                IsOpenId = true
            };

            using (var session = testDb.SessionFactory.OpenSession())
            {
                using (var tx = session.BeginTransaction())
                {
                    await session.SaveAsync(new DeviceFlowCodes()
                    {
                        DeviceCode = testDeviceCode,
                        ID = testUserCode,
                        ClientId = existingDeviceCode.ClientId,
                        CreationTime = existingDeviceCode.CreationTime,
                        Expiration = existingDeviceCode.CreationTime.AddSeconds(existingDeviceCode.Lifetime),
                        Data = serializer.Serialize(existingDeviceCode)
                    });
                    await tx.CommitAsync();
                }
            }

            using (var session = testDb.SessionFactory.OpenSession())
            {
                var store = new DeviceFlowStore(session, serializer, loggerMock.Object);
                await store.RemoveByDeviceCodeAsync(testDeviceCode);
            }

            using (var session = testDb.SessionFactory.OpenSession())
            {
                (await session.GetAsync<DeviceFlowCodes>(testUserCode)).Should().BeNull();
            }
        }

        [Theory]
        [MemberData(nameof(TestDatabases))]
        public async Task Removing_An_Unexisting_DeviceCode_Does_Not_Throw_Exception(TestDatabase testDb)
        {
            var loggerMock = new Mock<ILogger<DeviceFlowStore>>();
            var serializer = new PersistentGrantSerializer();

            using (var session = testDb.SessionFactory.OpenSession())
            {
                var store = new DeviceFlowStore(session, serializer, loggerMock.Object);

                Func<Task> act = async () => await store.RemoveByDeviceCodeAsync($"device_{Guid.NewGuid().ToString()}");

                await act.Should().NotThrowAsync();
            }
        }
    }
}