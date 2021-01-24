using System;
using System.Linq;
using System.Security.Claims;

namespace PaderConference.Infrastructure.Extensions
{
    public static class UserExtensions
    {
        public static string GetUserId(this ClaimsPrincipal principal)
        {
            return principal.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value ??
                   throw new InvalidOperationException("This claims principal has no id");
        }
    }
}
