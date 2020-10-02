using System.Security.Claims;

namespace PaderConference.Core.Interfaces.Services
{
    public interface IJwtValidator
    {
        ClaimsPrincipal? GetPrincipalFromToken(string token);
    }
}