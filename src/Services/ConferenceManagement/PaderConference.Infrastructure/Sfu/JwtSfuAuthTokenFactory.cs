using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PaderConference.Core.Services;
using PaderConference.Core.Services.Media.Gateways;

namespace PaderConference.Infrastructure.Sfu
{
    public class JwtSfuAuthTokenFactory : ISfuAuthTokenFactory
    {
        private readonly SfuJwtOptions _options;
        private readonly JwtSecurityTokenHandler _handler = new();

        public JwtSfuAuthTokenFactory(IOptions<SfuJwtOptions> options)
        {
            _options = options.Value;

            ThrowIfInvalidOptions(_options);
        }

        public ValueTask<string> GenerateToken(Participant participant, string connectionId)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, participant.Id),
                new Claim(SfuClaimTypes.ConferenceId, participant.ConferenceId),
                new Claim(SfuClaimTypes.ConnectionId, connectionId),
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = _options.Expiration.UtcDateTime,
                SigningCredentials = _options.SigningCredentials,
                Issuer = _options.Issuer,
                Audience = _options.Audience,
            };

            var token = _handler.CreateToken(tokenDescriptor);
            return new ValueTask<string>(_handler.WriteToken(token));
        }

        private static void ThrowIfInvalidOptions(SfuJwtOptions options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));

            if (options.SigningCredentials == null)
                throw new ArgumentNullException(nameof(SfuJwtOptions.SigningCredentials));


            if (options.ValidFor <= TimeSpan.Zero)
                throw new ArgumentException("Must be a non-zero TimeSpan.", nameof(SfuJwtOptions.ValidFor));
        }
    }
}
