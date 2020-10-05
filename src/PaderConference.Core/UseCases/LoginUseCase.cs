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
    public class LoginUseCase : UseCaseStatus<LoginResponse>, ILoginUseCase
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

        public async ValueTask<LoginResponse?> Handle(LoginRequest message)
        {
            if (string.IsNullOrEmpty(message.UserName))
                return ReturnError(
                    new FieldValidationError(nameof(message.UserName), "The username must not be empty."));

            string accessToken;
            if (message.IsGuestAuth)
            {
                accessToken = await _jwtFactory.GenerateUserToken(message.UserName);
            }
            else
            {
                if (string.IsNullOrEmpty(message.Password))
                    return ReturnError(
                        new FieldValidationError(nameof(message.Password), "The password must not be empty."));

                if (!UserDb.TryGetValue(message.UserName!, out var userData))
                    return ReturnError(AuthenticationError.UserNotFound.SetField(nameof(message.UserName)));

                var (password, userId) = userData;

                if (message.Password != password)
                    return ReturnError(
                        new AuthenticationError("The password is invalid.", ErrorCode.InvalidPassword).SetField(
                            nameof(message.Password)));

                accessToken =
                    await _jwtFactory.GenerateModeratorToken(userId.ToString(), "Vincent@me.de", message.UserName);
            }

            //// generate refresh token
            var refreshToken = _tokenFactory.GenerateToken();

            return new LoginResponse(accessToken, refreshToken);
        }
    }
}