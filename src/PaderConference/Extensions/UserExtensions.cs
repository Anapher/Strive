using System;
using System.Security.Claims;

namespace PaderConference.Extensions
{
    public static class UserExtensions
    {
        public static string GetUserId(this ClaimsPrincipal principal)
        {
            return principal.Identity.Name ?? throw new InvalidOperationException("This claims principal has no id");
        }
    }
}