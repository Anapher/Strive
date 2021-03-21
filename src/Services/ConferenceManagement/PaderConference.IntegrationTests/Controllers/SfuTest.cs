using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using PaderConference.IntegrationTests._Helpers;
using PaderConference.Messaging.SFU.Dto;
using Xunit;
using Xunit.Abstractions;

namespace PaderConference.IntegrationTests.Controllers
{
    [Collection(IntegrationTestCollection.Definition)]
    public class SfuTest : ServiceIntegrationTest
    {
        private const string ApiKey = "testApiKey";

        public SfuTest(MongoDbFixture mongoDb, ITestOutputHelper testOutputHelper) : base(testOutputHelper, mongoDb)
        {
        }

        [Fact]
        public async Task Fetch_NoApiKey_ReturnForbid()
        {
            // arrange
            var response = await Client.GetAsync("/v1/sfu/123");

            // assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task Fetch_InvalidApiKey_ReturnForbid()
        {
            // arrange
            var response = await Client.GetAsync("/v1/sfu/123?apiKey=wtf");

            // assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task Fetch_CorrectApiKeyConferenceNotExist_ReturnEmpty()
        {
            // arrange
            var response = await Client.GetAsync($"/v1/sfu/123?apiKey={ApiKey}");

            // assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var result = await response.Content.ReadFromJsonAsync<SfuConferenceInfo>();
            Assert.NotNull(result);

            Assert.Empty(result!.Permissions);
            Assert.Empty(result!.ParticipantToRoom);
        }

        [Fact]
        public async Task Fetch_UserJoined_NotEmpty()
        {
            // arrange
            var (_, conference) = await ConnectToOpenedConference();

            var response = await Client.GetAsync($"/v1/sfu/{conference.ConferenceId}?apiKey={ApiKey}");

            // assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var result = await response.Content.ReadFromJsonAsync<SfuConferenceInfo>();
            Assert.NotNull(result);

            Assert.NotEmpty(result!.Permissions);
            Assert.NotEmpty(result!.ParticipantToRoom);
        }
    }
}
