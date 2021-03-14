using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using PaderConference.Core.Services.BreakoutRooms;
using PaderConference.Core.Services.Rooms;
using PaderConference.Hubs;
using PaderConference.Hubs.Dtos;
using PaderConference.IntegrationTests._Helpers;
using Xunit;
using Xunit.Abstractions;

namespace PaderConference.IntegrationTests.Services
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
                SynchronizedBreakoutRoomsProvider.SyncObjId, value =>
                {
                    Assert.NotNull(value.Active);

                    Assert.Equal(description, value.Active?.Description);
                    Assert.Null(value.Active?.Deadline);
                    Assert.Equal(amount, value.Active?.Amount);
                });

            await connection.SyncObjects.AssertSyncObject<SynchronizedRooms>(
                SynchronizedRoomsProvider.SynchronizedObjectId, value =>
                {
                    Assert.Equal(amount + 1, value.Rooms.Count);
                });
        }
    }
}
