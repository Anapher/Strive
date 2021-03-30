using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServerHost.Quickstart.UI;

namespace Identity.API
{
    public class ProfileService : IProfileService
    {
        public Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            //>Processing
            var user = TestUsers.Users.First(x =>
                x.SubjectId == context.Subject.Claims.First(x => x.Type == JwtClaimTypes.Subject).Value);

            var claims = new List<Claim>
            {
                user.Claims.First(x => x.Type == JwtClaimTypes.Role),
                user.Claims.First(x => x.Type == JwtClaimTypes.Name),
            };

            context.IssuedClaims.AddRange(claims);

            return Task.CompletedTask;
        }

        public Task IsActiveAsync(IsActiveContext context)
        {
            //>Processing
            context.IsActive = true;
            return Task.CompletedTask;
        }
    }
}
