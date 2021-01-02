using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using PaderConference.Core.Domain;
using PaderConference.Core.Dto.UseCaseRequests;
using PaderConference.Core.Dto.UseCaseResponses;
using PaderConference.Core.Errors;
using PaderConference.Core.Interfaces;
using PaderConference.Core.Interfaces.Gateways.Repositories;
using PaderConference.Core.Interfaces.Services;
using PaderConference.Core.Interfaces.UseCases;

namespace PaderConference.Core.UseCases
{
    public class ExchangeRefreshTokenUseCase : IExchangeRefreshTokenUseCase
    {
        private readonly IJwtFactory _jwtFactory;
        private readonly IJwtValidator _jwtValidator;
        private readonly IRefreshTokenFactory _tokenFactory;
        private readonly IRefreshTokenRepo _refreshTokenRepo;
        private readonly IAuthService _authService;

        public ExchangeRefreshTokenUseCase(IJwtValidator jwtValidator, IJwtFactory jwtFactory,
            IRefreshTokenFactory tokenFactory, IRefreshTokenRepo refreshTokenRepo, IAuthService authService)
        {
            _jwtValidator = jwtValidator;
            _jwtFactory = jwtFactory;
            _tokenFactory = tokenFactory;
            _refreshTokenRepo = refreshTokenRepo;
            _authService = authService;
        }

        public async ValueTask<SuccessOrError<ExchangeRefreshTokenResponse>> Handle(ExchangeRefreshTokenRequest message)
        {
            var claimsPrincipal = _jwtValidator.GetPrincipalFromToken(message.AccessToken);
            if (claimsPrincipal == null)
                // invalid token/signing key was passed and we can't extract user claims
                return AuthenticationError.InvalidToken;

            var name = claimsPrincipal.Claims.First(x => x.Type == ClaimTypes.Name).Value;
            var role = claimsPrincipal.Claims.First(x => x.Type == ClaimTypes.Role).Value;
            var id = claimsPrincipal.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value;

            var token = await _refreshTokenRepo.TryPopRefreshToken(id, message.RefreshToken);
            if (token == null)
                return AuthenticationError.InvalidToken;

            if (!token.Active)
                return AuthenticationError.TokenExpired;

            string accessToken;
            if (role == PrincipalRoles.Guest)
            {
                accessToken = await _jwtFactory.GenerateGuestToken(name, id);
            }
            else
            {
                var user = await _authService.FindUser(name);
                if (user == null) return AuthenticationError.UserNotFound;

                accessToken = await _jwtFactory.GenerateModeratorToken(user.Id, name);
            }

            var newToken = _tokenFactory.Create(id);
            await _refreshTokenRepo.PushRefreshToken(newToken);

            return new ExchangeRefreshTokenResponse(accessToken, newToken.Value);
        }
    }
}