using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using PaderConference.Models.Request;
using PaderConference.Models.Response;

namespace PaderConference.IntegrationTests._Helpers
{
    public static class AuthHelper
    {
        public static async Task<LoginResponseDto> Login(HttpClient client)
        {
            var loginResponse = await client.PostAsync("/api/v1/auth/login",
                new JsonContent(new LoginRequestDto
                {
                    UserName = CustomWebApplicationFactory.USERNAME,
                    Password = CustomWebApplicationFactory.PASSWORD,
                }));
            loginResponse.EnsureSuccessStatusCode();

            return await loginResponse.Content.ReadFromJsonAsync<LoginResponseDto>();
        }
    }
}
