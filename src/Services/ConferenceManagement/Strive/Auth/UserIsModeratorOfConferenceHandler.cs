using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Strive.Core.Domain.Entities;
using Strive.Infrastructure.Extensions;

namespace Strive.Auth
{
    public class
        UserIsModeratorOfConferenceHandler : AuthorizationHandler<OperationAuthorizationRequirement, Conference>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
            OperationAuthorizationRequirement requirement, Conference resource)
        {
            if (context.User.IsInRole(AppRoles.Administrator))
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            var userId = context.User.GetUserId();
            if (resource.Configuration.Moderators.Contains(userId))
                context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }
}
