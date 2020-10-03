using System;
using System.Collections.Generic;
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

        public ValueTask<string> GenerateModeratorToken(string id, string email, string name)
        {
            return GenerateEncodedToken(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, id),
                new Claim(ClaimTypes.Name, name),
                new Claim(ClaimTypes.Role, Constants.Strings.JwtRoles.Moderator),
                new Claim(ClaimTypes.Email, email)
            });
        }

        public ValueTask<string> GenerateUserToken(string name)
        {
            return GenerateEncodedToken(new[]
            {
                new Claim(ClaimTypes.Name, name),
                new Claim(ClaimTypes.Role, Constants.Strings.JwtRoles.User)
            });
        }

        private ValueTask<string> GenerateEncodedToken(IEnumerable<Claim> claims)
        {
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
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