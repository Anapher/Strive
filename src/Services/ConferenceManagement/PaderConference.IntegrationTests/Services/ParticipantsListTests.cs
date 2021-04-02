using System.Threading.Tasks;
using PaderConference.Core.Services.ParticipantsList;
using PaderConference.Core.Services.Synchronization;
using PaderConference.IntegrationTests._Helpers;
using Xunit;
using Xunit.Abstractions;

namespace PaderConference.IntegrationTests.Services
{
    [Collection(IntegrationTestCollection.Definition)]
    public class ParticipantsListTests : ServiceIntegrationTest
    {
        private static readonly SynchronizedObjectId SyncObjId = SynchronizedParticipants.SyncObjId;

        public ParticipantsListTests(ITestOutputHelper testOutputHelper, MongoDbFixture mongoDb) : base(
            testOutputHelper, mongoDb)
        {
        }

        [Fact]
        public async Task Join_AddParticipantToList_UpdateSynchronizedObject()
        {
            // arrange
            var (connection, _) = await ConnectToOpenedConference();

            // assert
            await connection.SyncObjects.AssertSyncObject<SynchronizedParticipants>(SyncObjId,
                value => Assert.Contains(value.Participants,
                    x => x.Key == connection.User.Sub && x.Value.DisplayName == connection.User.Name &&
                         x.Value.IsModerator == connection.User.IsModerator));
        }

        [Fact]
        public async Task Join_AddAnotherParticipant_UpdateSynchronizedObject()
        {
            // arrange
            var (connection, conference) = await ConnectToOpenedConference();

            var olaf = CreateUser();

            // act
            await ConnectUserToConference(olaf, conference);

            // assert
            await connection.SyncObjects.AssertSyncObject<SynchronizedParticipants>(SyncObjId,
                value => Assert.Contains(value.Participants, x => x.Key == olaf.Sub));
        }

        [Fact]
        public async Task Join_ParticipantLeaves_UpdateSynchronizedObject()
        {
            // arrange
            var (connection, conference) = await ConnectToOpenedConference();

            var olaf = CreateUser();
            var olafConnection = await ConnectUserToConference(olaf, conference);

            await connection.SyncObjects.AssertSyncObject<SynchronizedParticipants>(SyncObjId,
                value => Assert.Contains(value.Participants, x => x.Key == olaf.Sub));

            // act
            await olafConnection.Hub.StopAsync();

            // assert
            await connection.SyncObjects.AssertSyncObject<SynchronizedParticipants>(SyncObjId,
                value => Assert.DoesNotContain(value.Participants, x => x.Key == olaf.Sub));
        }
    }
}
