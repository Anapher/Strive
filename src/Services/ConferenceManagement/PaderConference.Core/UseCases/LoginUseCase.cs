using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PaderConference.Core.Dto.UseCaseRequests;
using PaderConference.Core.Dto.UseCaseResponses;
using PaderConference.Core.Errors;
using PaderConference.Core.Interfaces;
using PaderConference.Core.Interfaces.Gateways.Repositories;
using PaderConference.Core.Interfaces.Services;
using PaderConference.Core.Interfaces.UseCases;

namespace PaderConference.Core.UseCases
{
    public class LoginUseCase : ILoginUseCase
    {
        private readonly IJwtFactory _jwtFactory;
        private readonly IRefreshTokenFactory _tokenFactory;
        private readonly IAuthService _authService;
        private readonly IRefreshTokenRepo _refreshTokenRepo;

        public LoginUseCase(IJwtFactory jwtFactory, IRefreshTokenFactory tokenFactory, IAuthService authService,
            IRefreshTokenRepo refreshTokenRepo)
        {
            _jwtFactory = jwtFactory;
            _tokenFactory = tokenFactory;
            _authService = authService;
            _refreshTokenRepo = refreshTokenRepo;
        }

        public async ValueTask<SuccessOrError<LoginResponse>> Handle(LoginRequest message)
        {
            string accessToken;
            string userId;
            if (message.IsGuestAuth)
            {
                userId = Guid.NewGuid().ToString("N");
                accessToken = await _jwtFactory.GenerateGuestToken(message.UserName, userId);
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
                        Fields = new Dictionary<string, string> {{nameof(message.Password), "Invalid password."}},
                    };

                accessToken = await _jwtFactory.GenerateModeratorToken(user.Id, message.UserName);
                userId = user.Id;
            }

            // generate refresh token
            var refreshToken = _tokenFactory.Create(userId);
            await _refreshTokenRepo.PushRefreshToken(refreshToken);

            return new LoginResponse(accessToken, refreshToken.Value);
        }
    }
}