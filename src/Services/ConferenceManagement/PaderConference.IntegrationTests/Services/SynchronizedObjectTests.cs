using System.Threading.Tasks;
using PaderConference.Core.Services.Synchronization;
using PaderConference.IntegrationTests._Helpers;
using Xunit;
using Xunit.Abstractions;

namespace PaderConference.IntegrationTests.Services
{
    [Collection(IntegrationTestCollection.Definition)]
    public class SynchronizedObjectTests : ServiceIntegrationTest
    {
        public SynchronizedObjectTests(ITestOutputHelper testOutputHelper, MongoDbFixture mongoDb) : base(
            testOutputHelper, mongoDb)
        {
        }

        [Fact]
        public async Task Join_ConferenceOpen_ReceiveSubscriptions()
        {
            var syncObjId = SynchronizedSubscriptions.SyncObjId(Moderator.Sub);

            // arrange
            var (connection, _) = await ConnectToOpenedConference();

            // act
            var subscriptions = await connection.SyncObjects.WaitForSyncObj<SynchronizedSubscriptions>(syncObjId);

            // assert
            Assert.NotEmpty(subscriptions.Subscriptions);
        }
    }
}
