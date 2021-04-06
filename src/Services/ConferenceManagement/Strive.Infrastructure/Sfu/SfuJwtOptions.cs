using System;
using Microsoft.IdentityModel.Tokens;

namespace Strive.Infrastructure.Sfu
{
    public class SfuJwtOptions
    {
        /// <summary>
        ///     The signing key to use when generating tokens.
        /// </summary>
        public SigningCredentials? SigningCredentials { get; set; }

        /// <summary>
        ///     4.1.4.  "exp" (Expiration Time) Claim - The "exp" (expiration time) claim identifies the expiration time on or
        ///     after which the JWT MUST NOT be accepted for processing.
        /// </summary>
        public DateTimeOffset Expiration => IssuedAt.Add(ValidFor);

        /// <summary>
        ///     4.1.6.  "iat" (Issued At) Claim - The "iat" (issued at) claim identifies the time at which the JWT was issued.
        /// </summary>
        public DateTimeOffset IssuedAt => DateTimeOffset.UtcNow;

        /// <summary>
        ///     Set the timespan the token will be valid for (default is 2 days)
        /// </summary>
        public TimeSpan ValidFor { get; set; } = TimeSpan.FromDays(2);

        /// <summary>
        ///     4.1.1.  "iss" (Issuer) Claim - The "iss" (issuer) claim identifies the principal that issued the JWT.
        /// </summary>
        public string? Issuer { get; set; }

        /// <summary>
        ///     4.1.3.  "aud" (Audience) Claim - The "aud" (audience) claim identifies the recipients that the JWT is intended for.
        /// </summary>
        public string? Audience { get; set; }
    }
}
