//using System.Collections.Generic;
//using System.Collections.Immutable;
//using System.Net;
//using System.Net.Http;
//using System.Net.Http.Json;
//using System.Threading.Tasks;
//using Newtonsoft.Json.Linq;
//using PaderConference.Core.Domain.Entities;
//using PaderConference.Core.Dto;
//using PaderConference.Core.Errors;
//using PaderConference.IntegrationTests._Helpers;
//using PaderConference.Models.Request;
//using PaderConference.Models.Response;
//using Xunit;
//using JsonContent = PaderConference.IntegrationTests._Helpers.JsonContent;

//namespace PaderConference.IntegrationTests.Controllers
//{
//    public class CreateConferenceTest : IClassFixture<CustomWebApplicationFactory>
//    {
//        private readonly HttpClient _client;
//        private readonly UserLogin _login;

//        public CreateConferenceTest(CustomWebApplicationFactory factory)
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
//        public async Task CreateConference_ValidConfig_Created()
//        {
//            var authResponse = await AuthHelper.Login(_client, _login);
//            AuthHelper.SetupHttpClient(authResponse, _client);

//            var response = await _client.PostAsync("/api/v1/conference", new JsonContent(GetValidConference()));

//            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
//            var result = await response.Content.ReadFromJsonAsync<StartConferenceResponseDto>();

//            Assert.NotNull(result);
//            Assert.NotEmpty(result.ConferenceId);
//        }

//        [Fact]
//        public async Task CreateConference_NotAuthorized_ReturnError()
//        {
//            var response = await _client.PostAsync("/api/v1/conference", new JsonContent(GetValidConference()));
//            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
//        }

//        [Fact]
//        public async Task CreateConference_WithoutModerators_ReturnError()
//        {
//            var authResponse = await AuthHelper.Login(_client, _login);
//            AuthHelper.SetupHttpClient(authResponse, _client);

//            var config = GetValidConference();
//            config.Configuration.Moderators = ImmutableList<string>.Empty;

//            var response = await _client.PostAsync("/api/v1/conference", new JsonContent(config));
//            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

//            var error = await response.Content.ReadFromJsonAsync<Error>();
//            Assert.NotNull(error);
//            Assert.Equal(ErrorType.BadRequest.ToString(), error.Type);
//            Assert.Equal(ErrorCode.FieldValidation.ToString(), error.Code);
//        }
//    }
//}

