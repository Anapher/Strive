using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using PaderConference.Core.Interfaces;
using PaderConference.Core.Services.Media;
using PaderConference.Hubs;
using PaderConference.IntegrationTests._Helpers;
using Xunit;
using Xunit.Abstractions;

namespace PaderConference.IntegrationTests.Services
{
    [Collection(IntegrationTestCollection.Definition)]
    public class MediaTests : ServiceIntegrationTest
    {
        public MediaTests(ITestOutputHelper testOutputHelper, MongoDbFixture mongoDb) : base(testOutputHelper, mongoDb)
        {
        }

        [Fact]
        public async Task FetchSfuConnectionInfo_ClosedConference_ReturnInfo()
        {
            // arrange
            var conference = await CreateConference();
            var connection = await ConnectUserToConference(Moderator, conference);

            // act
            var result =
                await connection.Hub.InvokeAsync<SuccessOrError<SfuConnectionInfo>>(
                    nameof(CoreHub.FetchSfuConnectionInfo));

            // assert
            AssertSuccess(result);
            Assert.NotNull(result.Response!.AuthToken);
            Assert.NotNull(result.Response!.Url);
        }
    }
}
