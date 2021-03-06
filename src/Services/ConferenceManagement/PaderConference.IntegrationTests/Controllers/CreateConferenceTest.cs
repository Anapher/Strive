using System.Collections.Generic;
using System.Collections.Immutable;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using PaderConference.Core.Domain.Entities;
using PaderConference.Core.Dto;
using PaderConference.Core.Errors;
using PaderConference.Models.Request;
using PaderConference.Models.Response;
using Xunit;

namespace PaderConference.IntegrationTests.Controllers
{
    public class CreateConferenceTest : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly CustomWebApplicationFactory _factory;
        private readonly HttpClient _client;

        public CreateConferenceTest(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            _client = factory.GetClient();
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
        public async Task CreateConference_NotModerator_ReturnForbidden()
        {
            _factory.CreateUser("Vincent", false).SetupHttpClient(_client);

            var response = await _client.PostAsync("/v1/conference", JsonContent.Create(GetValidConference()));
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
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
            Assert.NotNull(error);
            Assert.Equal(ErrorType.BadRequest.ToString(), error!.Type);
            Assert.Equal(ErrorCode.FieldValidation.ToString(), error.Code);
        }
    }
}

