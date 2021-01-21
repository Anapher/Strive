namespace PaderConference.Core.Dto.UseCaseResponses
{
    public class ExchangeRefreshTokenResponse
    {
        public string AccessToken { get; }
        public string RefreshToken { get; }

        public ExchangeRefreshTokenResponse(string accessToken, string refreshToken)
        {
            AccessToken = accessToken;
            RefreshToken = refreshToken;
        }
    }
}
