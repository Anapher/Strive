using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using PaderConference.Infrastructure.Interfaces;

namespace PaderConference.Infrastructure.Auth
{
    internal sealed class JwtHandler : IJwtHandler
    {
        private readonly JwtSecurityTokenHandler _jwtSecurityTokenHandler;
        private readonly ILogger _logger;

        public JwtHandler(ILogger<JwtHandler> logger)
        {
            _logger = logger;
            _jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
        }

        public string WriteToken(SecurityTokenDescriptor jwt)
        {
            var token = _jwtSecurityTokenHandler.CreateToken(jwt);

            return _jwtSecurityTokenHandler.WriteToken(token);
        }

        public ClaimsPrincipal? ValidateToken(string token, TokenValidationParameters tokenValidationParameters)
        {
            try
            {
                var principal =
                    _jwtSecurityTokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);

                if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                    !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                        StringComparison.InvariantCultureIgnoreCase))
                    throw new SecurityTokenException("Invalid token");

                return principal;
            }
            catch (Exception e)
            {
                _logger.LogError($"Token validation failed: {e.Message}");
                return null;
            }
        }
    }
}
