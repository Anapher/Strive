using System;
using Microsoft.Extensions.Options;
using PaderConference.Core.Domain.Entities;
using PaderConference.Core.Interfaces.Services;

namespace PaderConference.Infrastructure.Auth
{
    public class RefreshTokenFactory : IRefreshTokenFactory
    {
        private readonly ITokenFactory _tokenFactory;
        private readonly AuthSettings _settings;

        public RefreshTokenFactory(ITokenFactory tokenFactory, IOptions<AuthSettings> settings)
        {
            _tokenFactory = tokenFactory;
            _settings = settings.Value;
        }

        public RefreshToken Create(string userId)
        {
            var token = _tokenFactory.GenerateToken();
            var expiresAt = DateTimeOffset.UtcNow.Add(_settings.RefreshTokenValidFor);

            return new RefreshToken(userId, token, expiresAt);
        }
    }
}
