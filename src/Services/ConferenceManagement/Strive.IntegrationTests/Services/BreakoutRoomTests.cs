using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json.Linq;
using Strive.Core.Domain.Entities;
using Strive.Core.Extensions;
using Strive.Core.Interfaces;
using Strive.Core.Services.BreakoutRooms;
using Strive.Core.Services.Permissions;
using Strive.Core.Services.Rooms;
using Strive.Hubs.Core;
using Strive.Hubs.Core.Dtos;
using Strive.IntegrationTests._Helpers;
using Strive.Models.Request;
using Xunit;
using Xunit.Abstractions;

namespace Strive.IntegrationTests.Services
{
    [Collection(IntegrationTestCollection.Definition)]
    public class BreakoutRoomTests : ServiceIntegrationTest
    {
        public BreakoutRoomTests(ITestOutputHelper testOutputHelper, MongoDbFixture mongoDb) : base(testOutputHelper,
            mongoDb)
        {
        }

        [Fact]
        public async Task OpenBreakoutRooms_NoAssignments_UpdateSyncObjAndCreateRooms()
        {
            const string description = "hello world";
            const int amount = 5;

            // arrange
            var (connection, _) = await ConnectToOpenedConference();

            // act
            await connection.Hub.InvokeAsync(nameof(CoreHub.OpenBreakoutRooms),
                new OpenBreakoutRoomsDto(amount, null, description, null));

            // assert
            await connection.SyncObjects.AssertSyncObject<SynchronizedBreakoutRooms>(
                SynchronizedBreakoutRooms.SyncObjId, value =>
                {
                    Assert.NotNull(value.Active);

                    Assert.Equal(description, value.Active?.Description);
                    Assert.Null(value.Active?.Deadline);
                    Assert.Equal(amount, value.Active?.Amount);
                });

            await connection.SyncObjects.AssertSyncObject<SynchronizedRooms>(SynchronizedRooms.SyncObjId,
                value => { Assert.Equal(amount + 1, value.Rooms.Count); });
        }

        [Fact]
        public async Task OpenBreakoutRooms_MoveToBreakoutRoom_UpdatePermissions()
        {
            const int amount = 1;

            var permission = DefinedPermissions.Media.CanChangeOtherParticipantsProducers;

            // arrange
            var conferenceCreationDto = new CreateConferenceRequestDto
            {
                Configuration =
                    new ConferenceConfiguration {Moderators = Moderator.Yield().Select(x => x.Sub).ToList()},
                Permissions = new Dictionary<PermissionType, Dictionary<string, JValue>>
                {
                    {
                        PermissionType.Moderator,
                        new Dictionary<string, JValue>(permission.Configure(false).Yield())
                    },
                    {
                        PermissionType.BreakoutRoom,
                        new Dictionary<string, JValue>(permission.Configure(true).Yield())
                    },
                },
            };

            var conference = await CreateConference(conferenceCreationDto);
            var connection = await ConnectUserToConference(Moderator, conference);
            AssertSuccess(await OpenConference(connection));

            Task AssertPermission(bool value)
            {
                return connection.SyncObjects.AssertSyncObject<SynchronizedParticipantPermissions>(
                    SynchronizedParticipantPermissions.SyncObjId(Moderator.Sub),
                    syncObj => { Assert.Contains(permission.Configure(value), syncObj.Permissions); });
            }

            AssertSuccess(await connection.Hub.InvokeAsync<SuccessOrError<Unit>>(nameof(CoreHub.OpenBreakoutRooms),
                new OpenBreakoutRoomsDto(amount, null, null, null)));

            await AssertPermission(false);

            await connection.SyncObjects.AssertSyncObject<SynchronizedRooms>(SynchronizedRooms.SyncObjId,
                value => Assert.Equal(2, value.Rooms.Count));
            var syncRooms =
                connection.SyncObjects.GetSynchronizedObject<SynchronizedRooms>(SynchronizedRooms.SyncObjId);

            var breakoutRoom = syncRooms.Rooms.Single(x => x.RoomId != syncRooms.DefaultRoomId);

            // act
            AssertSuccess(await connection.Hub.InvokeAsync<SuccessOrError<Unit>>(nameof(CoreHub.SwitchRoom),
                new SwitchRoomDto(breakoutRoom.RoomId)));

            // assert
            await AssertPermission(true);
        }

        [Fact]
        public async Task OpenBreakoutRooms_AssignParticipants_MoveParticipantsToRooms()
        {
            const string description = "hello world";
            const int amount = 2;

            // arrange
            var (connection, conference) = await ConnectToOpenedConference();

            var user1 = CreateUser();
            var user2 = CreateUser();

            await ConnectUserToConference(user1, conference);
            await ConnectUserToConference(user2, conference);

            var assignments = new[] {new[] {user1.Sub}, new[] {user2.Sub}};

            // act
            await connection.Hub.InvokeAsync(nameof(CoreHub.OpenBreakoutRooms),
                new OpenBreakoutRoomsDto(amount, null, description, assignments));

            // assert
            await connection.SyncObjects.AssertSyncObject<SynchronizedRooms>(SynchronizedRooms.SyncObjId, value =>
            {
                Assert.Equal(value.Participants[user1.Sub], value.Rooms.First(x => x.DisplayName == "Alpha").RoomId);
                Assert.Equal(value.Participants[user2.Sub], value.Rooms.First(x => x.DisplayName == "Bravo").RoomId);
            });
        }

        [Fact]
        public async Task OpenBreakoutRooms_WithDeadline_AutomaticallyCloseBreakoutRooms()
        {
            const string description = "hello world";
            const int amount = 2;

            // arrange
            var (connection, _) = await ConnectToOpenedConference();

            // act
            var deadline = DateTimeOffset.UtcNow.AddMilliseconds(200);
            await connection.Hub.InvokeAsync(nameof(CoreHub.OpenBreakoutRooms),
                new OpenBreakoutRoomsDto(amount, deadline, description, null));

            // assert
            await connection.SyncObjects.AssertSyncObject<SynchronizedBreakoutRooms>(
                SynchronizedBreakoutRooms.SyncObjId, value => { Assert.NotNull(value.Active); });
            await connection.SyncObjects.AssertSyncObject<SynchronizedBreakoutRooms>(
                SynchronizedBreakoutRooms.SyncObjId, value => { Assert.Null(value.Active); });
        }

        [Fact]
        public async Task CloseBreakoutRooms_NotOpened_NoError()
        {
            // arrange
            var (connection, _) = await ConnectToOpenedConference();

            // act
            var result = await connection.Hub.InvokeAsync<SuccessOrError<Unit>>(nameof(CoreHub.CloseBreakoutRooms));

            // assert
            AssertSuccess(result);
        }

        [Fact]
        public async Task CloseBreakoutRooms_Opened_UpdateSyncObj()
        {
            // arrange
            var (connection, _) = await ConnectToOpenedConference();

            await connection.Hub.InvokeAsync(nameof(CoreHub.OpenBreakoutRooms),
                new OpenBreakoutRoomsDto(5, null, "hello world", null));
            await connection.SyncObjects.AssertSyncObject<SynchronizedBreakoutRooms>(
                SynchronizedBreakoutRooms.SyncObjId, value => { Assert.NotNull(value.Active); });

            // act
            var result = await connection.Hub.InvokeAsync<SuccessOrError<Unit>>(nameof(CoreHub.CloseBreakoutRooms));

            // assert
            AssertSuccess(result);

            await connection.SyncObjects.AssertSyncObject<SynchronizedBreakoutRooms>(
                SynchronizedBreakoutRooms.SyncObjId, value => { Assert.Null(value.Active); });
        }

        [Fact]
        public async Task CloseBreakoutRooms_Opened_CloseRooms()
        {
            // arrange
            var (connection, _) = await ConnectToOpenedConference();

            await connection.Hub.InvokeAsync(nameof(CoreHub.OpenBreakoutRooms), DefaultConfig);
            await connection.SyncObjects.AssertSyncObject<SynchronizedRooms>(SynchronizedRooms.SyncObjId,
                value => { Assert.True(value.Rooms.Count > 1); });

            // act
            var result = await connection.Hub.InvokeAsync<SuccessOrError<Unit>>(nameof(CoreHub.CloseBreakoutRooms));

            // assert
            AssertSuccess(result);

            await connection.SyncObjects.AssertSyncObject<SynchronizedRooms>(SynchronizedRooms.SyncObjId,
                value => { Assert.Equal(1, value.Rooms.Count); });
        }

        private static readonly OpenBreakoutRoomsDto DefaultConfig = new(4, null, null, null);

        [Fact]
        public async Task PatchBreakoutRooms_IncreaseRooms_CreateRooms()
        {
            // arrange
            var (connection, _) = await ConnectToOpenedConference();

            var result =
                await connection.Hub.InvokeAsync<SuccessOrError<Unit>>(nameof(CoreHub.OpenBreakoutRooms),
                    DefaultConfig);
            AssertSuccess(result);

            var patch = new JsonPatchDocument<BreakoutRoomsConfig>();
            patch.Add(x => x.Amount, DefaultConfig.Amount + 2);

            // act
            result = await connection.Hub.InvokeAsync<SuccessOrError<Unit>>(nameof(CoreHub.ChangeBreakoutRooms), patch);

            // assert
            AssertSuccess(result);

            await connection.SyncObjects.AssertSyncObject<SynchronizedRooms>(SynchronizedRooms.SyncObjId,
                value => { Assert.Equal(DefaultConfig.Amount + 3, value.Rooms.Count); });
        }

        [Fact]
        public async Task PatchBreakoutRooms_DecreaseRooms_RemoveRooms()
        {
            // arrange
            var (connection, _) = await ConnectToOpenedConference();

            var result =
                await connection.Hub.InvokeAsync<SuccessOrError<Unit>>(nameof(CoreHub.OpenBreakoutRooms),
                    DefaultConfig);
            AssertSuccess(result);

            var patch = new JsonPatchDocument<BreakoutRoomsConfig>();
            patch.Add(x => x.Amount, DefaultConfig.Amount - 2);

            // act
            result = await connection.Hub.InvokeAsync<SuccessOrError<Unit>>(nameof(CoreHub.ChangeBreakoutRooms), patch);

            // assert
            AssertSuccess(result);

            await connection.SyncObjects.AssertSyncObject<SynchronizedRooms>(SynchronizedRooms.SyncObjId,
                value => { Assert.Equal(DefaultConfig.Amount - 1, value.Rooms.Count); });
        }

        [Fact]
        public async Task PatchBreakoutRooms_ChangeDescription_UpdateSyncObj()
        {
            const string description = "yo wtf";

            // arrange
            var (connection, _) = await ConnectToOpenedConference();

            var result =
                await connection.Hub.InvokeAsync<SuccessOrError<Unit>>(nameof(CoreHub.OpenBreakoutRooms),
                    DefaultConfig);
            AssertSuccess(result);

            var patch = new JsonPatchDocument<BreakoutRoomsConfig>();
            patch.Add(x => x.Description, description);

            // act
            result = await connection.Hub.InvokeAsync<SuccessOrError<Unit>>(nameof(CoreHub.ChangeBreakoutRooms), patch);

            // assert
            AssertSuccess(result);

            await connection.SyncObjects.AssertSyncObject<SynchronizedBreakoutRooms>(
                SynchronizedBreakoutRooms.SyncObjId,
                value => { Assert.Equal(description, value.Active?.Description); });
        }

        [Fact]
        public async Task PatchBreakoutRooms_CreateDeadline_CloseRoomsAfterDeadline()
        {
            // arrange
            var (connection, _) = await ConnectToOpenedConference();

            var result =
                await connection.Hub.InvokeAsync<SuccessOrError<Unit>>(nameof(CoreHub.OpenBreakoutRooms),
                    DefaultConfig);
            AssertSuccess(result);

            await connection.SyncObjects.AssertSyncObject<SynchronizedBreakoutRooms>(
                SynchronizedBreakoutRooms.SyncObjId, value => { Assert.NotNull(value.Active); });

            var patch = new JsonPatchDocument<BreakoutRoomsConfig>();
            patch.Add(x => x.Deadline, DateTimeOffset.UtcNow.AddMilliseconds(200));

            // act
            result = await connection.Hub.InvokeAsync<SuccessOrError<Unit>>(nameof(CoreHub.ChangeBreakoutRooms), patch);

            // assert
            AssertSuccess(result);

            await connection.SyncObjects.AssertSyncObject<SynchronizedBreakoutRooms>(
                SynchronizedBreakoutRooms.SyncObjId, value => { Assert.Null(value.Active); });
        }
    }
}
