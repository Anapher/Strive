//using System.Collections.Generic;
//using System.Collections.Immutable;
//using System.Net;
//using System.Net.Http;
//using System.Net.Http.Json;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.JsonPatch;
//using Newtonsoft.Json.Linq;
//using Strive.Core.Domain.Entities;
//using Strive.IntegrationTests._Helpers;
//using Strive.Models.Request;
//using Strive.Models.Response;
//using Xunit;
//using JsonContent = Strive.IntegrationTests._Helpers.JsonContent;

//namespace Strive.IntegrationTests.Controllers
//{
//    public class ConferenceLinkTest : IClassFixture<CustomWebApplicationFactory>
//    {
//        private readonly HttpClient _client;
//        private readonly UserLogin _login;

//        public ConferenceLinkTest(CustomWebApplicationFactory factory)
//        {
//            _client = factory.CreateClient();
//            _login = factory.CreateLogin();
//        }

//        private static CreateConferenceRequestDto GetValidConference()
//        {
//            return new()
//            {
//                Configuration = new ConferenceConfiguration {Moderators = new[] {"0"}.ToImmutableList()},
//                Permissions = new Dictionary<PermissionType, Dictionary<string, JValue>>(),
//            };
//        }

//        [Fact]
//        public async Task GetConferenceLinks_Unauthorized_ReturnError()
//        {
//            var response = await _client.GetAsync("/api/v1/conference-link");
//            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
//        }

//        [Fact]
//        public async Task GetConferenceLinks_NoLinksCreated_ReturnEmptyList()
//        {
//            var authResponse = await AuthHelper.Login(_client, _login);
//            AuthHelper.SetupHttpClient(authResponse, _client);

//            var response = await _client.GetAsync("/api/v1/conference-link");
//            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

//            var result = await response.Content.ReadFromJsonAsync<ConferenceLinkDto[]>();
//            Assert.NotNull(result);
//            Assert.Empty(result);
//        }

//        [Fact]
//        public async Task GetConferenceLinks_AfterConferenceCreated_ReturnListWithConference()
//        {
//            var authResponse = await AuthHelper.Login(_client, _login);
//            AuthHelper.SetupHttpClient(authResponse, _client);

//            var response = await _client.PostAsync("/api/v1/conference", new JsonContent(GetValidConference()));
//            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

//            var conferenceResult = await response.Content.ReadFromJsonAsync<StartConferenceResponseDto>();

//            var res = await _client.GetAsync("/api/v1/conference-link");
//            Assert.Equal(HttpStatusCode.OK, res.StatusCode);

//            var result = await res.Content.ReadFromJsonAsync<SerializableConferenceLinkDto[]>();
//            Assert.NotNull(result);

//            var dto = Assert.Single(result);
//            var dtoConferenceId = dto.ConferenceId;
//            Assert.Equal(conferenceResult.ConferenceId, dtoConferenceId);
//        }

//        [Fact]
//        public async Task PatchConferenceLink_ValidPatchDocument_ConferenceLinkChanged()
//        {
//            var authResponse = await AuthHelper.Login(_client, _login);
//            AuthHelper.SetupHttpClient(authResponse, _client);

//            var response = await _client.PostAsync("/api/v1/conference", new JsonContent(GetValidConference()));
//            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

//            var res = await _client.GetAsync("/api/v1/conference-link");
//            Assert.Equal(HttpStatusCode.OK, res.StatusCode);

//            var result = await res.Content.ReadFromJsonAsync<SerializableConferenceLinkDto[]>();
//            var dto = Assert.Single(result);

//            Assert.False(dto.Starred);

//            var patch = new JsonPatchDocument<SerializableConferenceLinkDto>();
//            patch.Replace(x => x.Starred, true);

//            var patchRes =
//                await _client.PatchAsync($"/api/v1/conference-link/{dto.ConferenceId}", new JsonContent(patch));
//            Assert.Equal(HttpStatusCode.OK, patchRes.StatusCode);

//            res = await _client.GetAsync("/api/v1/conference-link");
//            result = await res.Content.ReadFromJsonAsync<SerializableConferenceLinkDto[]>();
//            dto = Assert.Single(result);

//            Assert.True(dto.Starred);
//        }

//        public class SerializableConferenceLinkDto
//        {
//            public string ConferenceId { get; set; }
//            public bool Starred { get; set; }
//        }
//    }
//}

