namespace PaderConference.Models.Response
{
    public class ExchangeRefreshTokenResponseDto
    {
        public string AccessToken { get; }
        public string RefreshToken { get; }

        public ExchangeRefreshTokenResponseDto(string accessToken, string refreshToken)
        {
            AccessToken = accessToken;
            RefreshToken = refreshToken;
        }
    }
}
