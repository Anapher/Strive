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
        private static readonly IReadOnlyDictionary<string, (string, int)> UserDb =
            new Dictionary<string, (string, int)>
            {
                {
                    "Vincent", ("123", 1)
                }
            };

        private readonly IJwtFactory _jwtFactory;
        private readonly ITokenFactory _tokenFactory;

        public LoginUseCase(IJwtFactory jwtFactory, ITokenFactory tokenFactory)
        {
            _jwtFactory = jwtFactory;
            _tokenFactory = tokenFactory;
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
                if (!UserDb.TryGetValue(message.UserName, out var userData))
                    return AuthenticationError.UserNotFound with
                    {
                        Fields = new Dictionary<string, string> {{nameof(message.UserName), "User not found"}},
                    };

                var (password, userId) = userData;

                if (message.Password != password)
                    return AuthenticationError.InvalidPassword with
                    {
                        Fields = new Dictionary<string, string> {{nameof(message.UserName), "Invalid password."}},
                    };

                accessToken =
                    await _jwtFactory.GenerateModeratorToken(userId.ToString(), "Vincent@me.de", message.UserName);
            }

            //// generate refresh token
            var refreshToken = _tokenFactory.GenerateToken();

            return new LoginResponse(accessToken, refreshToken);
        }
    }
}