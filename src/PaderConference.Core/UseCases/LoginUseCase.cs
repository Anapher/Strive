using System.Collections.Generic;
using System.Threading.Tasks;
using PaderConference.Core.Dto.UseCaseRequests;
using PaderConference.Core.Dto.UseCaseResponses;
using PaderConference.Core.Errors;
using PaderConference.Core.Interfaces;
using PaderConference.Core.Interfaces.Services;
using PaderConference.Core.Interfaces.UseCases;

namespace PaderConference.Core.UseCases
{
    public class LoginUseCase : ILoginUseCase
    {
        private readonly IJwtFactory _jwtFactory;
        private readonly ITokenFactory _tokenFactory;
        private readonly IAuthService _authService;

        public LoginUseCase(IJwtFactory jwtFactory, ITokenFactory tokenFactory, IAuthService authService)
        {
            _jwtFactory = jwtFactory;
            _tokenFactory = tokenFactory;
            _authService = authService;
        }

        public async ValueTask<SuccessOrError<LoginResponse>> Handle(LoginRequest message)
        {
            string accessToken;
            if (message.IsGuestAuth)
            {
                accessToken = await _jwtFactory.GenerateGuestToken(message.UserName, null);
            }
            else
            {
                var user = await _authService.FindUser(message.UserName);
                if (user == null)
                    return AuthenticationError.UserNotFound with
                    {
                        Fields = new Dictionary<string, string> {{nameof(message.UserName), "User not found"}},
                    };

                if (!await user.ValidatePassword(message.Password))
                    return AuthenticationError.InvalidPassword with
                    {
                        Fields = new Dictionary<string, string> {{nameof(message.UserName), "Invalid password."}},
                    };

                accessToken = await _jwtFactory.GenerateModeratorToken(user.Id, message.UserName);
            }

            // generate refresh token
            var refreshToken = _tokenFactory.GenerateToken();

            return new LoginResponse(accessToken, refreshToken);
        }
    }
}