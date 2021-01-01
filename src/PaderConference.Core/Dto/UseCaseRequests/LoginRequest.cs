using System.Diagnostics.CodeAnalysis;
using PaderConference.Core.Dto.UseCaseResponses;
using PaderConference.Core.Interfaces;

namespace PaderConference.Core.Dto.UseCaseRequests
{
    public class LoginRequest : IUseCaseRequest<LoginResponse>
    {
        public LoginRequest(string userName, string password, string? remoteIpAddress)
        {
            UserName = userName;
            Password = password;
            RemoteIpAddress = remoteIpAddress;
            IsGuestAuth = false;
        }

        public LoginRequest(string userName, string? remoteIpAddress)
        {
            UserName = userName;
            RemoteIpAddress = remoteIpAddress;
            IsGuestAuth = true;
        }

        public string UserName { get; }

        public string? Password { get; }

        public string? RemoteIpAddress { get; }

        [MemberNotNullWhen(false, nameof(Password))]
        public bool IsGuestAuth { get; }
    }
}