using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Strive.Core.Domain.Entities;
using Strive.Core.Dto;
using Strive.Core.Errors;
using Strive.Models.Request;
using Strive.Models.Response;
using Xunit;
using Xunit.Abstractions;

namespace Strive.IntegrationTests.Controllers
{
    [Collection(IntegrationTestCollection.Definition)]
    public class CreateConferenceTest
    {
        private readonly CustomWebApplicationFactory _factory;
        private readonly HttpClient _client;

        public CreateConferenceTest(MongoDbFixture mongoDb, ITestOutputHelper testOutputHelper)
        {
            _factory = new CustomWebApplicationFactory(mongoDb, testOutputHelper);
            _client = _factory.CreateClient();
        }

        private static CreateConferenceRequestDto GetValidConference()
        {
            return new()
            {
                Configuration = new ConferenceConfiguration {Moderators = new List<string> {"0"}},
                Permissions = new Dictionary<PermissionType, Dictionary<string, JValue>>(),
            };
        }

        [Fact]
        public async Task CreateConference_ValidConfig_Created()
        {
            _factory.CreateUser("Vincent", true).SetupHttpClient(_client);
            var response = await _client.PostAsync("/v1/conference", JsonContent.Create(GetValidConference()));

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var result = await response.Content.ReadFromJsonAsync<ConferenceCreatedResponseDto>();

            Assert.NotNull(result?.ConferenceId);
        }

        [Fact]
        public async Task CreateConference_NotAuthorized_ReturnUnauthorized()
        {
            var response = await _client.PostAsync("/v1/conference", JsonContent.Create(GetValidConference()));
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task CreateConference_WithoutModerators_ReturnValidationError()
        {
            _factory.CreateUser("Ralph", true).SetupHttpClient(_client);

            var conference = GetValidConference();
            conference.Configuration.Moderators = ImmutableList<string>.Empty;

            var response = await _client.PostAsync("/v1/conference", JsonContent.Create(conference));
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            var error = await response.Content.ReadFromJsonAsync<Error>();
            Assert.Equal(ErrorType.BadRequest.ToString(), error?.Type);
            Assert.Equal(ErrorCode.FieldValidation.ToString(), error?.Code);

            Assert.All(error?.Fields?.Keys ?? Enumerable.Empty<string>(), x => Assert.True(char.IsLower(x[0])));
        }
    }
}
