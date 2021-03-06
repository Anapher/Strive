using System.Threading.Tasks;
using PaderConference.Core.Services.Rooms;
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
        public async Task Join_ConferenceOpened_RoomMappingEmpty()
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
    }
}
