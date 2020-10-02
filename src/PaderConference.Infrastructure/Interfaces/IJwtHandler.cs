using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace PaderConference.Infrastructure.Interfaces
{
    public interface IJwtHandler
    {
        string WriteToken(SecurityTokenDescriptor jwt);
        ClaimsPrincipal? ValidateToken(string token, TokenValidationParameters tokenValidationParameters);
    }
}