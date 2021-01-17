using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using PaderConference.Models.Request;
using PaderConference.Models.Response;

namespace PaderConference.IntegrationTests._Helpers
{
    public static class AuthHelper
    {
        public static async Task<LoginResponseDto> Login(HttpClient client, UserLogin login)
        {
            var loginResponse = await client.PostAsync("/api/v1/auth/login",
                new JsonContent(new LoginRequestDto {UserName = login.Name, Password = login.Password}));
            loginResponse.EnsureSuccessStatusCode();

            return await loginResponse.Content.ReadFromJsonAsync<LoginResponseDto>();
        }

        public static void SetupHttpClient(LoginResponseDto dto, HttpClient client)
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", dto.AccessToken);
        }
    }
}
