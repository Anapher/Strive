using PaderConference.Core.Dto.UseCaseRequests;
using PaderConference.Core.Dto.UseCaseResponses;
using PaderConference.Core.Errors;
using PaderConference.Core.Interfaces;
using PaderConference.Core.Interfaces.Gateways.Repositories;
using PaderConference.Core.Interfaces.Services;
using PaderConference.Core.Interfaces.UseCases;
using System.Threading.Tasks;

namespace PaderConference.Core.UseCases
{
    public class LoginUseCase : UseCaseStatus<LoginResponse>, ILoginUseCase
    {
        private readonly IJwtFactory _jwtFactory;
        private readonly ITokenFactory _tokenFactory;

        public LoginUseCase(IJwtFactory jwtFactory, ITokenFactory tokenFactory)
        {
            _jwtFactory = jwtFactory;
            _tokenFactory = tokenFactory;
        }

        public async Task<LoginResponse?> Handle(LoginRequest message)
        {
            if (string.IsNullOrEmpty(message.UserName))
                return ReturnError(new FieldValidationError(nameof(message.UserName), "The username must not be empty."));

            if (string.IsNullOrEmpty(message.Password))
                return ReturnError(new FieldValidationError(nameof(message.Password), "The password must not be empty."));

            //var user = await _userRepository.FindByName(message.UserName);
            //if (user == null)
            //    return ReturnError(AuthenticationError.UserNotFound.SetField(nameof(message.UserName)));

            //if (!await _userRepository.CheckPassword(user, message.Password))
            //    return ReturnError(new AuthenticationError("The password is invalid.", ErrorCode.InvalidPassword).SetField(nameof(message.Password)));

            //// generate refresh token
            var refreshToken = _tokenFactory.GenerateToken();
            //user.AddRefreshToken(refreshToken, message.RemoteIpAddress);
            //await _userRepository.Update(user);

            var accessToken = await _jwtFactory.GenerateEncodedToken("123", "Vincent");

            return new LoginResponse(accessToken, refreshToken);
        }
    }
}
