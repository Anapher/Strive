using Autofac;
using PaderConference.Core.Services.Permissions;
using PaderConference.Infrastructure.ServiceFactories.Base;

namespace PaderConference.Infrastructure.ServiceFactories
{
    public class PermissionsServiceManager : AutowiredConferenceServiceManager<PermissionsService>
    {
        public PermissionsServiceManager(IComponentContext context) : base(context)
        {
        }
    }
}