using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;

namespace Strive.IntegrationTests._Helpers
{
    public class MockJwtTokens
    {
        private readonly JwtSecurityTokenHandler _tokenHandler = new();
        private readonly RandomNumberGenerator _rng = RandomNumberGenerator.Create();
        private readonly byte[] _key = new byte[32];

        public MockJwtTokens()
        {
            _rng.GetBytes(_key);
            SecurityKey = new SymmetricSecurityKey(_key) {KeyId = Guid.NewGuid().ToString()};
            SigningCredentials = new SigningCredentials(SecurityKey, SecurityAlgorithms.HmacSha256);
        }

        public string Issuer { get; } = Guid.NewGuid().ToString();
        public SecurityKey SecurityKey { get; }
        public SigningCredentials SigningCredentials { get; }

        public string GenerateJwtToken(IEnumerable<Claim> claims)
        {
            return _tokenHandler.WriteToken(new JwtSecurityToken(Issuer, null, claims, null,
                DateTime.UtcNow.AddMinutes(20), SigningCredentials));
        }
    }
}
