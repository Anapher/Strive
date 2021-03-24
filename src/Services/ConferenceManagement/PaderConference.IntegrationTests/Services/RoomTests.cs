using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.SignalR.Client;
using PaderConference.Core.Interfaces;
using PaderConference.Core.Services.Rooms;
using PaderConference.Core.Services.Synchronization;
using PaderConference.Hubs.Core;
using PaderConference.Hubs.Core.Dtos;
using PaderConference.IntegrationTests._Helpers;
using Xunit;
using Xunit.Abstractions;

namespace PaderConference.IntegrationTests.Services
{
    [Collection(IntegrationTestCollection.Definition)]
    public class RoomTests : ServiceIntegrationTest
    {
        private static readonly SynchronizedObjectId SyncObjId = SynchronizedRoomsProvider.SynchronizedObjectId;

        public RoomTests(ITestOutputHelper testOutputHelper, MongoDbFixture mongoDb) : base(testOutputHelper, mongoDb)
        {
        }

        [Fact]
        public async Task Join_ConferenceClosed_RoomMappingEmpty()
        {
            // arrange
            var conference = await CreateConference();
            var connection = await ConnectUserToConference(Moderator, conference);

            // assert
            var syncObj = await connection.SyncObjects.WaitForSyncObj<SynchronizedRooms>(SyncObjId);

            Assert.Empty(syncObj.Participants);
            Assert.Empty(syncObj.Rooms);
        }

        [Fact]
        public async Task Join_ConferenceOpened_RoomMappingUpdated()
        {
            // arrange
            var (connection, _) = await ConnectToOpenedConference();

            // assert
            await connection.SyncObjects.AssertSyncObject<SynchronizedRooms>(SyncObjId, value =>
            {
                Assert.Single(value.Participants);
                Assert.Single(value.Rooms);
            });
        }

        [Fact]
        public async Task CreateRooms_ConferenceOpened_AddRoomsToSyncObjects()
        {
            // arrange
            var (connection, _) = await ConnectToOpenedConference();

            // act
            var result = await connection.Hub.InvokeAsync<SuccessOrError<IReadOnlyList<Room>>>(
                nameof(CoreHub.CreateRooms), new List<RoomCreationInfo> {new("Test1"), new("Test2")});

            // assert
            AssertSuccess(result);
            Assert.Equal(2, result.Response!.Count);

            await connection.SyncObjects.AssertSyncObject<SynchronizedRooms>(SyncObjId, value =>
            {
                Assert.Equal(3, value.Rooms.Count);
                Assert.Contains(value.Rooms, x => x.DisplayName == "Test1");
                Assert.Contains(value.Rooms, x => x.DisplayName == "Test2");
            });
        }

        [Fact]
        public async Task SwitchRoom_ConferenceOpened_UpdateSynchronizedObject()
        {
            // arrange
            var (connection, _) = await ConnectToOpenedConference();

            var roomCreationResponse = await connection.Hub.InvokeAsync<SuccessOrError<IReadOnlyList<Room>>>(
                nameof(CoreHub.CreateRooms), new List<RoomCreationInfo> {new("Test Room")});
            var createdRoom = Assert.Single(roomCreationResponse.Response!);

            // act
            var result = await connection.Hub.InvokeAsync<SuccessOrError<Unit>>(nameof(CoreHub.SwitchRoom),
                new SwitchRoomDto(createdRoom!.RoomId));

            // assert
            AssertSuccess(result);

            await connection.SyncObjects.AssertSyncObject<SynchronizedRooms>(SyncObjId, value =>
            {
                var mapping = Assert.Single(value.Participants);
                Assert.Equal(connection.User.Sub, mapping.Key);
                Assert.Equal(createdRoom.RoomId, mapping.Value);
            });
        }

        [Fact]
        public async Task RemoveRooms_RoomsAreCreated_UpdateSynchronizedObject()
        {
            // arrange
            var (connection, _) = await ConnectToOpenedConference();

            var roomCreationResponse = await connection.Hub.InvokeAsync<SuccessOrError<IReadOnlyList<Room>>>(
                nameof(CoreHub.CreateRooms), new List<RoomCreationInfo> {new("Test1")});
            var createdRoom = Assert.Single(roomCreationResponse.Response!);

            // act
            var result = await connection.Hub.InvokeAsync<SuccessOrError<Unit>>(nameof(CoreHub.RemoveRooms),
                new[] {createdRoom.RoomId});

            // assert
            AssertSuccess(result);

            await connection.SyncObjects.AssertSyncObject<SynchronizedRooms>(SyncObjId,
                value => { Assert.Single(value.Rooms); });
        }
    }
}
