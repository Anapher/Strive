using Autofac;
using PaderConference.Core.Services.Permissions;

namespace PaderConference.Infrastructure.Services.Permissions
{
    public class PermissionsServiceManager : AutowiredConferenceServiceManager<PermissionsService>
    {
        public PermissionsServiceManager(IComponentContext context) : base(context)
        {
        }
    }
}