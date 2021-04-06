using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json.Linq;
using Strive.Core.Interfaces;
using Strive.Core.Services;
using Strive.Core.Services.Permissions;
using Strive.Core.Services.Permissions.Responses;
using Strive.Hubs.Core;
using Strive.Hubs.Core.Dtos;
using Strive.IntegrationTests._Helpers;
using Xunit;
using Xunit.Abstractions;

namespace Strive.IntegrationTests.Services
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
                SynchronizedParticipantPermissions.SyncObjId(connection.User.Sub));

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

            var syncObjId = SynchronizedParticipantPermissions.SyncObjId(testUser.Sub);

            await testUserConnection.SyncObjects.AssertSyncObject<SynchronizedParticipantPermissions>(syncObjId,
                value => Assert.DoesNotContain(value.Permissions, x => x.Key == permission.Key));

            // act
            var result = await connection.Hub.InvokeAsync<SuccessOrError<Unit>>(nameof(CoreHub.SetTemporaryPermission),
                new SetTemporaryPermissionDto(testUser.Sub, permission.Key, (JValue) JToken.FromObject(true)));

            // assert
            AssertSuccess(result);

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

            var result = await connection.Hub.InvokeAsync<SuccessOrError<Unit>>(nameof(CoreHub.SetTemporaryPermission),
                new SetTemporaryPermissionDto(testUser.Sub, permission.Key, (JValue) JToken.FromObject(true)));
            AssertSuccess(result);

            // act
            result = await connection.Hub.InvokeAsync<SuccessOrError<Unit>>(nameof(CoreHub.SetTemporaryPermission),
                new SetTemporaryPermissionDto(testUser.Sub, permission.Key, null));

            // assert
            AssertSuccess(result);

            var syncObjId = SynchronizedParticipantPermissions.SyncObjId(testUser.Sub);
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
            var result = await testUserConnection.Hub.InvokeAsync<SuccessOrError<Unit>>(
                nameof(CoreHub.SetTemporaryPermission),
                new SetTemporaryPermissionDto(testUser.Sub, permission.Key, (JValue) JToken.FromObject(true)));

            // assert
            AssertFailed(result);

            var syncObjId = SynchronizedParticipantPermissions.SyncObjId(testUser.Sub);
            await testUserConnection.SyncObjects.AssertSyncObject<SynchronizedParticipantPermissions>(syncObjId,
                value => Assert.DoesNotContain(value.Permissions, x => x.Key == permission.Key));
        }

        [Fact]
        public async Task FetchPermissions_NotModeratorAndFetchOwnPermissions_ReturnMyPermissions()
        {
            // arrange
            var (_, conference) = await ConnectToOpenedConference();

            var testUser = CreateUser();
            var testUserConnection = await ConnectUserToConference(testUser, conference);

            // act
            var result =
                await testUserConnection.Hub.InvokeAsync<SuccessOrError<ParticipantPermissionResponse>>(
                    nameof(CoreHub.FetchPermissions), null);

            // assert
            AssertSuccess(result);

            Assert.NotEmpty(result.Response!.Layers);
            Assert.Equal(testUser.Sub, result.Response!.ParticipantId);
        }

        [Fact]
        public async Task FetchPermissions_NotModeratorAndFetchOthersPermissions_ReturnError()
        {
            // arrange
            var (_, conference) = await ConnectToOpenedConference();

            var testUser = CreateUser();
            var testUserConnection = await ConnectUserToConference(testUser, conference);

            // act
            var result =
                await testUserConnection.Hub.InvokeAsync<SuccessOrError<ParticipantPermissionResponse>>(
                    nameof(CoreHub.FetchPermissions), Moderator.Sub);

            // assert
            AssertFailed(result);
            AssertErrorCode(ServiceErrorCode.PermissionDenied, result.Error!);
        }

        [Fact]
        public async Task FetchPermissions_ModeratorAndFetchOthersPermissions_ReturnPermissions()
        {
            // arrange
            var (connection, conference) = await ConnectToOpenedConference();

            var testUser = CreateUser();
            await ConnectUserToConference(testUser, conference);

            // act
            var result =
                await connection.Hub.InvokeAsync<SuccessOrError<ParticipantPermissionResponse>>(
                    nameof(CoreHub.FetchPermissions), testUser.Sub);

            // assert
            AssertSuccess(result);
            Assert.NotEmpty(result.Response!.Layers);
        }
    }
}
