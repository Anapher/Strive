using System.Collections.Generic;
using System.Collections.Immutable;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using PaderConference.Core.Domain.Entities;
using PaderConference.IntegrationTests._Helpers;
using PaderConference.Models.Request;
using PaderConference.Models.Response;
using Xunit;
using JsonContent = PaderConference.IntegrationTests._Helpers.JsonContent;

namespace PaderConference.IntegrationTests.Controllers
{
    public class ConferenceLinkTest : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;
        private readonly UserLogin _login;

        public ConferenceLinkTest(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
            _login = factory.CreateLogin();
        }

        private static CreateConferenceRequestDto GetValidConference()
        {
            return new()
            {
                Configuration = new ConferenceConfiguration {Moderators = new[] {"0"}.ToImmutableList()},
                Permissions = new Dictionary<PermissionType, Dictionary<string, JValue>>(),
            };
        }

        [Fact]
        public async Task GetConferenceLinks_Unauthorized_ReturnError()
        {
            var response = await _client.GetAsync("/api/v1/conference-links");
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task GetConferenceLinks_NoLinksCreated_ReturnEmptyList()
        {
            var authResponse = await AuthHelper.Login(_client, _login);
            AuthHelper.SetupHttpClient(authResponse, _client);

            var response = await _client.GetAsync("/api/v1/conference-links");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var result = await response.Content.ReadFromJsonAsync<ConferenceLinkDto[]>();
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetConferenceLinks_AfterConferenceCreated_ReturnListWithConference()
        {
            var authResponse = await AuthHelper.Login(_client, _login);
            AuthHelper.SetupHttpClient(authResponse, _client);

            var response = await _client.PostAsync("/api/v1/conference", new JsonContent(GetValidConference()));
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var conferenceResult = await response.Content.ReadFromJsonAsync<StartConferenceResponseDto>();

            var res = await _client.GetAsync("/api/v1/conference-links");
            Assert.Equal(HttpStatusCode.OK, res.StatusCode);

            var result = await res.Content.ReadFromJsonAsync<SerializableConferenceLinkDto[]>();
            Assert.NotNull(result);

            var dto = Assert.Single(result);
            var dtoConferenceId = dto.ConferenceId;
            Assert.Equal(conferenceResult.ConferenceId, dtoConferenceId);
        }

        public class SerializableConferenceLinkDto
        {
            public string ConferenceId { get; set; }
        }
    }
}
