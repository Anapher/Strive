//using System.Net;
//using System.Net.Http;
//using System.Net.Http.Json;
//using System.Threading.Tasks;
//using Newtonsoft.Json.Linq;
//using PaderConference.IntegrationTests._Helpers;
//using PaderConference.Models.Request;
//using PaderConference.Models.Response;
//using Xunit;
//using JsonContent = PaderConference.IntegrationTests._Helpers.JsonContent;

//namespace PaderConference.IntegrationTests.Controllers
//{
//    public class AuthIntegrationTests : IClassFixture<CustomWebApplicationFactory>
//    {
//        private readonly HttpClient _client;
//        private readonly UserLogin _login;

//        public AuthIntegrationTests(CustomWebApplicationFactory factory)
//        {
//            _client = factory.CreateClient();
//            _login = factory.CreateLogin();
//        }

//        [Fact]
//        public async Task Login_HasValidCredentials_ReturnAccessToken()
//        {
//            var httpResponse = await _client.PostAsync("/api/v1/auth/login",
//                new JsonContent(new LoginRequestDto {UserName = _login.Name, Password = _login.Password}));
//            httpResponse.EnsureSuccessStatusCode();

//            var stringResponse = await httpResponse.Content.ReadAsStringAsync();
//            dynamic result = JObject.Parse(stringResponse);
//            Assert.NotNull(result.accessToken);
//            Assert.NotNull(result.refreshToken);
//        }

//        [Fact]
//        public async Task Login_HasInvalidCredentials_ReturnErrorCode()
//        {
//            var httpResponse = await _client.PostAsync("/api/v1/auth/login",
//                new JsonContent(new LoginRequestDto {UserName = "unknown", Password = "Rhcp1234"}));
//            var stringResponse = await httpResponse.Content.ReadAsStringAsync();
//            Assert.Contains("The user was not found.", stringResponse);
//            Assert.Equal(HttpStatusCode.BadRequest, httpResponse.StatusCode);
//        }

//        [Fact]
//        public async Task LoginAsGuest_ValidRequest_ReturnAccessToken()
//        {
//            var httpResponse = await _client.PostAsync("/api/v1/auth/guest",
//                new JsonContent(new AuthGuestRequestDto {DisplayName = "Vincent"}));
//            httpResponse.EnsureSuccessStatusCode();

//            var stringResponse = await httpResponse.Content.ReadAsStringAsync();
//            dynamic result = JObject.Parse(stringResponse);
//            Assert.NotNull(result.accessToken);
//            Assert.NotNull(result.refreshToken);
//        }

//        [Fact]
//        public async Task ExchangeRefreshToken_ValidRequest_ReturnNewToken()
//        {
//            var loginResponse = await AuthHelper.Login(_client, _login);

//            var httpResponse = await _client.PostAsync("/api/v1/auth/refreshtoken",
//                new JsonContent(new ExchangeRefreshTokenRequestDto
//                {
//                    AccessToken = loginResponse.AccessToken, RefreshToken = loginResponse.RefreshToken,
//                }));
//            httpResponse.EnsureSuccessStatusCode();

//            var newAuthInfo = await httpResponse.Content.ReadFromJsonAsync<LoginResponseDto>();
//            Assert.NotNull(newAuthInfo?.AccessToken);
//            Assert.NotNull(newAuthInfo.RefreshToken);

//            Assert.NotEqual(newAuthInfo.RefreshToken, loginResponse.RefreshToken);
//        }

//        [Fact]
//        public async Task ExchangeRefreshToken_RefreshTwice_SecondRefreshCausesError()
//        {
//            var loginResponse = await AuthHelper.Login(_client, _login);

//            var httpResponse = await _client.PostAsync("/api/v1/auth/refreshtoken",
//                new JsonContent(new ExchangeRefreshTokenRequestDto
//                {
//                    AccessToken = loginResponse.AccessToken, RefreshToken = loginResponse.RefreshToken,
//                }));
//            httpResponse.EnsureSuccessStatusCode();

//            httpResponse = await _client.PostAsync("/api/v1/auth/refreshtoken",
//                new JsonContent(new ExchangeRefreshTokenRequestDto
//                {
//                    AccessToken = loginResponse.AccessToken, RefreshToken = loginResponse.RefreshToken,
//                }));

//            Assert.NotEqual(HttpStatusCode.OK, httpResponse.StatusCode);
//        }

//        [Fact]
//        public async Task ExchangeRefreshToken_ExchangeTheExchangedToken_ReturnNewToken()
//        {
//            var loginResponse = await AuthHelper.Login(_client, _login);

//            var httpResponse = await _client.PostAsync("/api/v1/auth/refreshtoken",
//                new JsonContent(new ExchangeRefreshTokenRequestDto
//                {
//                    AccessToken = loginResponse.AccessToken, RefreshToken = loginResponse.RefreshToken,
//                }));
//            httpResponse.EnsureSuccessStatusCode();

//            loginResponse = await httpResponse.Content.ReadFromJsonAsync<LoginResponseDto>();

//            httpResponse = await _client.PostAsync("/api/v1/auth/refreshtoken",
//                new JsonContent(new ExchangeRefreshTokenRequestDto
//                {
//                    AccessToken = loginResponse.AccessToken, RefreshToken = loginResponse.RefreshToken,
//                }));

//            httpResponse.EnsureSuccessStatusCode();

//            var newAuthInfo = await httpResponse.Content.ReadFromJsonAsync<LoginResponseDto>();
//            Assert.NotNull(newAuthInfo?.AccessToken);
//            Assert.NotNull(newAuthInfo.RefreshToken);
//        }

//        [Fact]
//        public async Task ExchangeRefreshToken_InvalidRefreshToken_ResponseError()
//        {
//            var loginResponse = await AuthHelper.Login(_client, _login);

//            var httpResponse = await _client.PostAsync("/api/v1/auth/refreshtoken",
//                new JsonContent(new ExchangeRefreshTokenRequestDto
//                {
//                    AccessToken = loginResponse.AccessToken, RefreshToken = "unknown",
//                }));

//            Assert.NotEqual(HttpStatusCode.OK, httpResponse.StatusCode);

//            var stringResponse = await httpResponse.Content.ReadAsStringAsync();
//            Assert.Contains("Invalid token.", stringResponse);
//        }
//    }
//}

