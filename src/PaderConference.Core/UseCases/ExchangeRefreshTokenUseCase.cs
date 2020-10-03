using System.Linq;
using System.Threading.Tasks;
using PaderConference.Core.Dto.UseCaseRequests;
using PaderConference.Core.Dto.UseCaseResponses;
using PaderConference.Core.Errors;
using PaderConference.Core.Interfaces;
using PaderConference.Core.Interfaces.Services;
using PaderConference.Core.Interfaces.UseCases;

namespace PaderConference.Core.UseCases
{
    public class ExchangeRefreshTokenUseCase : UseCaseStatus<ExchangeRefreshTokenResponse>, IExchangeRefreshTokenUseCase
    {
        private readonly IJwtFactory _jwtFactory;
        private readonly IJwtValidator _jwtValidator;
        private readonly ITokenFactory _tokenFactory;

        public ExchangeRefreshTokenUseCase(IJwtValidator jwtValidator, IJwtFactory jwtFactory,
            ITokenFactory tokenFactory)
        {
            _jwtValidator = jwtValidator;
            _jwtFactory = jwtFactory;
            _tokenFactory = tokenFactory;
        }

        public async ValueTask<ExchangeRefreshTokenResponse?> Handle(ExchangeRefreshTokenRequest message)
        {
            var claimsPrincipal = _jwtValidator.GetPrincipalFromToken(message.AccessToken);
            if (claimsPrincipal == null)
                // invalid token/signing key was passed and we can't extract user claims
                return ReturnError(AuthenticationError.InvalidToken);

            var id = claimsPrincipal.Claims.First(x => x.Type == "id");
            //var user = await _userRepository.FindById(id.Value);
            //if (user == null)
            //    return ReturnError(AuthenticationError.UserNotFound);

            //if (!user.HasValidRefreshToken(message.RefreshToken))
            //    return ReturnError(AuthenticationError.InvalidToken);

            var jwToken = await _jwtFactory.GenerateModeratorToken("123", "Vincent", "Vincent");
            var refreshToken = _tokenFactory.GenerateToken();

            //user.RemoveRefreshToken(message.RefreshToken);
            //user.AddRefreshToken(refreshToken, message.RemoteIpAddress);

            //await _userRepository.Update(user);
            return new ExchangeRefreshTokenResponse(jwToken, refreshToken);
        }
    }
}