namespace PaderConference.Models.Response
{
    public class LoginResponseDto
    {
        public string AccessToken { get; }
        public string RefreshToken { get; }

        public LoginResponseDto(string accessToken, string refreshToken)
        {
            AccessToken = accessToken;
            RefreshToken = refreshToken;
        }
    }
}
