using System;

namespace PaderConference.Infrastructure.Auth
{
    public class AuthSettings
    {
        public string? SecretKey { get; set; }

        public TimeSpan RefreshTokenValidFor { get; set; } = TimeSpan.FromDays(30);
    }
}
