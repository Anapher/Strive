using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.JsonPatch;
using Newtonsoft.Json;
using Strive.Core;
using Strive.Core.Domain.Entities;
using Strive.Core.Dto;
using Strive.Core.Services.ConferenceManagement;
using Strive.Infrastructure.Serialization;
using Strive.IntegrationTests._Helpers;
using Strive.Models.Response;
using Strive.Tests.Utils;
using Xunit;
using Xunit.Abstractions;

namespace Strive.IntegrationTests.Controllers
{
    [Collection(IntegrationTestCollection.Definition)]
    public class PatchConferenceTest
    {
        private readonly CustomWebApplicationFactory _factory;
        private readonly HttpClient _client;

        public PatchConferenceTest(MongoDbFixture mongoDb, ITestOutputHelper testOutputHelper)
        {
            _factory = new CustomWebApplicationFactory(mongoDb, testOutputHelper);
            _client = _factory.CreateClient();
        }

        private static JsonPatchDocument<ConferenceData> GetValidPatch()
        {
            return new();
        }

        [Fact]
        public async Task PatchConference_DoesNotExist_ReturnConferenceNotFound()
        {
            _factory.CreateUser("Vincent", true).SetupHttpClient(_client);

            var patch = GetValidPatch();
            var response = await _client.PatchAsync("/v1/conference/asdasd", JsonNetContent.Create(patch));

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

            var result = await response.Content.ReadFromJsonAsync<Error>();
            Assert.Equal(ConferenceError.ConferenceNotFound.Code, result?.Code);
        }

        private async Task AssertConference(string conferenceId, Action<Conference> assertAction)
        {
            var response = await _client.GetAsync($"/v1/conference/{conferenceId}");
            response.EnsureSuccessStatusCode();

            var s = await response.Content.ReadAsStringAsync();
            var conference = JsonConvert.DeserializeObject<Conference>(s, JsonConfig.Default);

            Assert.NotNull(conference);

            assertAction(conference!);
        }

        private async Task<string> CreateConference(ConferenceData data)
        {
            var response = await _client.PostAsync("/v1/conference", JsonContent.Create(data));

            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<ConferenceCreatedResponseDto>();

            Assert.NotNull(result?.ConferenceId);
            return result!.ConferenceId;
        }

        [Fact]
        public async Task PatchConference_AddModerator_ReturnSuccess()
        {
            // arrange
            _factory.CreateUser("Vincent", true).SetupHttpClient(_client);

            var conferenceData = new ConferenceData();
            conferenceData.Configuration.Moderators.Add("test");

            var conferenceId = await CreateConference(conferenceData);

            // act
            var patch = new JsonPatchDocument<ConferenceData>();
            patch.Add(x => x.Configuration.Moderators, "test2");

            var response = await _client.PatchAsync($"/v1/conference/{conferenceId}", JsonNetContent.Create(patch));
            response.EnsureSuccessStatusCode();

            // assert
            await AssertConference(conferenceId,
                conference =>
                {
                    AssertHelper.AssertScrambledEquals(new[] {"test", "test2"}, conference.Configuration.Moderators);
                });
        }

        [Fact]
        public async Task PatchConference_RemoveModeratorSoNoModeratorsExist_ReturnErrorAndDontChange()
        {
            // arrange
            _factory.CreateUser("Vincent", true).SetupHttpClient(_client);

            var conferenceData = new ConferenceData();
            conferenceData.Configuration.Moderators.Add("test");

            var conferenceId = await CreateConference(conferenceData);

            // act
            var patch = new JsonPatchDocument<ConferenceData>();
            patch.Remove(x => x.Configuration.Moderators, 0);

            var response = await _client.PatchAsync($"/v1/conference/{conferenceId}", JsonNetContent.Create(patch));
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            // assert
            await AssertConference(conferenceId, conference => { Assert.Single(conference.Configuration.Moderators); });
        }
    }
}
