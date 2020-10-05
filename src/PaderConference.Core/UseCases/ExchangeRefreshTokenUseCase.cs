using System.Linq;
using System.Security.Claims;
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

            var name = claimsPrincipal.Claims.First(x => x.Type == ClaimTypes.Name).Value;

            var id = claimsPrincipal.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);
            if (id == null)
            {
                // guest
                var jwToken = await _jwtFactory.GenerateUserToken(name);
                var refreshToken = _tokenFactory.GenerateToken();

                return new ExchangeRefreshTokenResponse(jwToken, refreshToken);
            }
            else
            {
                var jwToken = await _jwtFactory.GenerateModeratorToken(id.Value, "test@test.de", name);
                var refreshToken = _tokenFactory.GenerateToken();

                return new ExchangeRefreshTokenResponse(jwToken, refreshToken);
            }
        }
    }
}