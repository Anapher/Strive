using System;
using System.Security.Cryptography;
using PaderConference.Core.Extensions;
using PaderConference.Core.Interfaces.Services;

namespace PaderConference.Infrastructure.Auth
{
    internal sealed class TokenFactory : ITokenFactory
    {
        public string GenerateToken(int size = 32)
        {
            var randomNumber = new byte[size];
            using var rng = RandomNumberGenerator.Create();

            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber).ToUrlBase64();
        }
    }
}
