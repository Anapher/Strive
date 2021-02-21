using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using MediatR;
using PaderConference.Core.Interfaces;
using PaderConference.Core.Services;
using PaderConference.Core.Services.Permissions;

namespace PaderConference.Hubs.Services.Middlewares
{
    public static class ServiceInvokerPermissionMiddleware
    {
        public static IServiceRequestBuilder<TResponse> RequirePermissions<TResponse>(
            this IServiceRequestBuilder<TResponse> builder, params PermissionDescriptor<bool>[] requiredPermissions)
        {
            return builder.AddMiddleware(context => CheckPermissions(context, requiredPermissions));
        }

        public static IServiceRequestBuilder<TResponse> RequirePermissions<TResponse>(
            this IServiceRequestBuilder<TResponse> builder, IEnumerable<PermissionDescriptor<bool>> requiredPermissions)
        {
            return builder.RequirePermissions(requiredPermissions.ToArray());
        }

        public static async ValueTask<SuccessOrError<Unit>> CheckPermissions(ServiceInvokerContext context,
            params PermissionDescriptor<bool>[] requiredPermissions)
        {
            if (requiredPermissions.Length == 0) return SuccessOrError<Unit>.Succeeded(Unit.Value);

            var participantPermissions = context.Context.Resolve<IParticipantPermissions>();
            var permissions =
                await participantPermissions.FetchForParticipant(context.Participant);

            foreach (var permission in requiredPermissions)
            {
                var permissionValue = await permissions.GetPermissionValue(permission);
                if (!permissionValue) return CommonError.PermissionDenied(permission);
            }

            return SuccessOrError<Unit>.Succeeded(Unit.Value);
        }
    }
}