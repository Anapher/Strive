using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.SignalR.Client;
using PaderConference.Core.Interfaces;
using PaderConference.Core.Services.Rooms;
using PaderConference.Hubs;
using PaderConference.Hubs.Dtos;
using PaderConference.IntegrationTests._Helpers;
using Xunit;
using Xunit.Abstractions;

namespace PaderConference.IntegrationTests.Controllers
{
    [Collection(IntegrationTestCollection.Definition)]
    public class RoomTests : ServiceIntegrationTest
    {
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
            var syncObj =
                await connection.SyncObjects.WaitForSyncObj<SynchronizedRooms>(SynchronizedRoomsProvider
                    .SynchronizedObjectId);

            Assert.Empty(syncObj.Participants);
            Assert.Empty(syncObj.Rooms);
        }

        [Fact]
        public async Task Join_ConferenceOpened_RoomMappingUpdated()
        {
            // arrange
            var (connection, _) = await ConnectToOpenedConference();

            // assert
            var syncObj =
                await connection.SyncObjects.WaitForSyncObj<SynchronizedRooms>(SynchronizedRoomsProvider
                    .SynchronizedObjectId);

            Assert.Single(syncObj.Participants);
            Assert.Single(syncObj.Rooms);
        }

        [Fact]
        public async Task CreateRooms_ConferenceOpened_AddRoomsToSyncObjects()
        {
            // arrange
            var (connection, _) = await ConnectToOpenedConference();

            // act
            var result = await connection.Connection.InvokeAsync<SuccessOrError<IReadOnlyList<Room>>>(
                nameof(CoreHub.CreateRooms), new List<RoomCreationInfo> {new("Test1"), new("Test2")});

            // assert
            Assert.True(result.Success);
            Assert.Equal(2, result.Response!.Count);

            var syncObj =
                await connection.SyncObjects.WaitForSyncObj<SynchronizedRooms>(SynchronizedRoomsProvider
                    .SynchronizedObjectId);

            Assert.Equal(3, syncObj.Rooms.Count);
            Assert.Contains(syncObj.Rooms, x => x.DisplayName == "Test1");
            Assert.Contains(syncObj.Rooms, x => x.DisplayName == "Test2");
        }

        [Fact]
        public async Task SwitchRoom_ConferenceOpened_UpdateSynchronizedObject()
        {
            // arrange
            var (connection, _) = await ConnectToOpenedConference();

            var roomCreationResponse = await connection.Connection.InvokeAsync<SuccessOrError<IReadOnlyList<Room>>>(
                nameof(CoreHub.CreateRooms), new List<RoomCreationInfo> {new("Test Room")});
            var createdRoom = Assert.Single(roomCreationResponse.Response!);

            // act
            var result = await connection.Connection.InvokeAsync<SuccessOrError<Unit>>(nameof(CoreHub.SwitchRoom),
                new SwitchRoomDto(createdRoom!.RoomId));

            // assert
            Assert.True(result.Success);

            var syncObj =
                await connection.SyncObjects.WaitForSyncObj<SynchronizedRooms>(SynchronizedRoomsProvider
                    .SynchronizedObjectId);

            var mapping = Assert.Single(syncObj.Participants);
            Assert.Equal(connection.User.Sub, mapping.Key);
            Assert.Equal(createdRoom.RoomId, mapping.Value);
        }

        [Fact]
        public async Task RemoveRooms_RoomsAreCreated_UpdateSynchronizedObject()
        {
            // arrange
            var (connection, _) = await ConnectToOpenedConference();

            var roomCreationResponse = await connection.Connection.InvokeAsync<SuccessOrError<IReadOnlyList<Room>>>(
                nameof(CoreHub.CreateRooms), new List<RoomCreationInfo> {new("Test1")});
            var createdRoom = Assert.Single(roomCreationResponse.Response!);

            // act
            var result = await connection.Connection.InvokeAsync<SuccessOrError<Unit>>(nameof(CoreHub.RemoveRooms),
                new[] {createdRoom.RoomId});

            // assert
            Assert.True(result.Success);

            var syncObj =
                await connection.SyncObjects.WaitForSyncObj<SynchronizedRooms>(SynchronizedRoomsProvider
                    .SynchronizedObjectId);

            Assert.Single(syncObj.Rooms);
        }
    }
}
