using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PaderConference.Core.Interfaces.Services;
using PaderConference.Infrastructure.Helpers;
using PaderConference.Infrastructure.Interfaces;

namespace PaderConference.Infrastructure.Auth
{
    public class JwtFactory : IJwtFactory
    {
        private readonly IJwtHandler _jwtHandler;
        private readonly JwtIssuerOptions _jwtOptions;

        public JwtFactory(IJwtHandler jwtHandler, IOptions<JwtIssuerOptions> options)
        {
            _jwtOptions = options.Value;
            _jwtHandler = jwtHandler;

            ThrowIfInvalidOptions(_jwtOptions);
        }

        public ValueTask<string> GenerateEncodedToken(string id, string email)
        {
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, id),
                    new Claim(ClaimTypes.Role, Constants.Strings.JwtRoles.Moderator),
                    new Claim(ClaimTypes.Email, email)
                }),
                Expires = _jwtOptions.Expiration.UtcDateTime,
                SigningCredentials = _jwtOptions.SigningCredentials,
                Issuer = _jwtOptions.Issuer,
                Audience = _jwtOptions.Audience,
                NotBefore = _jwtOptions.NotBefore.UtcDateTime,
                IssuedAt = _jwtOptions.IssuedAt.UtcDateTime
            };

            return new ValueTask<string>(_jwtHandler.WriteToken(tokenDescriptor));
        }

        private static void ThrowIfInvalidOptions(JwtIssuerOptions options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));

            if (options.ValidFor <= TimeSpan.Zero)
                throw new ArgumentException("Must be a non-zero TimeSpan.", nameof(JwtIssuerOptions.ValidFor));

            if (options.SigningCredentials == null)
                throw new ArgumentNullException(nameof(JwtIssuerOptions.SigningCredentials));

            if (options.JtiGenerator == null) throw new ArgumentNullException(nameof(JwtIssuerOptions.JtiGenerator));
        }
    }
}