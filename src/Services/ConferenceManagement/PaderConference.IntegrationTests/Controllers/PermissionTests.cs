using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json.Linq;
using PaderConference.Core.Interfaces;
using PaderConference.Core.Services.Permissions;
using PaderConference.Hubs;
using PaderConference.Hubs.Dtos;
using PaderConference.IntegrationTests._Helpers;
using Xunit;
using Xunit.Abstractions;

namespace PaderConference.IntegrationTests.Controllers
{
    [Collection(IntegrationTestCollection.Definition)]
    public class PermissionTests : ServiceIntegrationTest
    {
        public PermissionTests(ITestOutputHelper testOutputHelper, MongoDbFixture mongoDb) : base(testOutputHelper,
            mongoDb)
        {
        }

        [Fact]
        public async Task Join_DoNothing_SynchronizedObject()
        {
            // arrange
            var (connection, _) = await ConnectToOpenedConference();

            // assert
            var syncObj = await connection.SyncObjects.WaitForSyncObj<SynchronizedParticipantPermissions>(
                SynchronizedParticipantPermissionsProvider.GetObjIdOfParticipant(connection.User.Sub));

            Assert.NotEmpty(syncObj.Permissions);
        }

        [Fact]
        public async Task SetTemporaryPermissions_AddPermission_UpdateSynchronizedObject()
        {
            var permission = DefinedPermissions.Permissions.CanGiveTemporaryPermission;

            // arrange
            var (connection, conference) = await ConnectToOpenedConference();

            var testUser = CreateUser();
            var testUserConnection = await ConnectUserToConference(testUser, conference);

            var syncObjId = SynchronizedParticipantPermissionsProvider.GetObjIdOfParticipant(testUser.Sub);

            await testUserConnection.SyncObjects.AssertSyncObject<SynchronizedParticipantPermissions>(syncObjId,
                value => Assert.DoesNotContain(value.Permissions, x => x.Key == permission.Key));

            // act
            var result = await connection.Connection.InvokeAsync<SuccessOrError<Unit>>(
                nameof(CoreHub.SetTemporaryPermission),
                new SetTemporaryPermissionDto(testUser.Sub, permission.Key, (JValue) JToken.FromObject(true)));

            // assert
            Assert.True(result.Success);

            await testUserConnection.SyncObjects.AssertSyncObject<SynchronizedParticipantPermissions>(syncObjId,
                value => Assert.Contains(value.Permissions, x => x.Key == permission.Key));
        }

        [Fact]
        public async Task SetTemporaryPermissions_RemovePermission_UpdateSynchronizedObject()
        {
            var permission = DefinedPermissions.Permissions.CanGiveTemporaryPermission;

            // arrange
            var (connection, conference) = await ConnectToOpenedConference();

            var testUser = CreateUser();
            var testUserConnection = await ConnectUserToConference(testUser, conference);

            var result = await connection.Connection.InvokeAsync<SuccessOrError<Unit>>(
                nameof(CoreHub.SetTemporaryPermission),
                new SetTemporaryPermissionDto(testUser.Sub, permission.Key, (JValue) JToken.FromObject(true)));
            Assert.True(result.Success);

            // act
            result = await connection.Connection.InvokeAsync<SuccessOrError<Unit>>(
                nameof(CoreHub.SetTemporaryPermission),
                new SetTemporaryPermissionDto(testUser.Sub, permission.Key, null));

            // assert
            Assert.True(result.Success);

            var syncObjId = SynchronizedParticipantPermissionsProvider.GetObjIdOfParticipant(testUser.Sub);
            await testUserConnection.SyncObjects.AssertSyncObject<SynchronizedParticipantPermissions>(syncObjId,
                value => Assert.DoesNotContain(value.Permissions, x => x.Key == permission.Key));
        }

        [Fact]
        public async Task SetTemporaryPermissions_NotModerator_DontSetPermission()
        {
            var permission = DefinedPermissions.Permissions.CanGiveTemporaryPermission;

            // arrange
            var (_, conference) = await ConnectToOpenedConference();

            var testUser = CreateUser();
            var testUserConnection = await ConnectUserToConference(testUser, conference);

            // act
            var result = await testUserConnection.Connection.InvokeAsync<SuccessOrError<Unit>>(
                nameof(CoreHub.SetTemporaryPermission),
                new SetTemporaryPermissionDto(testUser.Sub, permission.Key, (JValue) JToken.FromObject(true)));

            // assert
            Assert.False(result.Success);

            var syncObjId = SynchronizedParticipantPermissionsProvider.GetObjIdOfParticipant(testUser.Sub);
            await testUserConnection.SyncObjects.AssertSyncObject<SynchronizedParticipantPermissions>(syncObjId,
                value => Assert.DoesNotContain(value.Permissions, x => x.Key == permission.Key));
        }
    }
}
