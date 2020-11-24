using Autofac;
using PaderConference.Core.Services.Rooms;
using PaderConference.Infrastructure.ServiceFactories.Base;

namespace PaderConference.Infrastructure.ServiceFactories
{
    public class RoomsServiceManager : AutowiredConferenceServiceManager<RoomsService>
    {
        public RoomsServiceManager(IComponentContext context) : base(context)
        {
        }
    }
}
