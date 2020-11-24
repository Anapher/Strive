using Autofac;
using PaderConference.Core.Services.BreakoutRoom;
using PaderConference.Infrastructure.ServiceFactories.Base;

namespace PaderConference.Infrastructure.ServiceFactories
{
    public class BreakoutRoomServiceManager : AutowiredConferenceServiceManager<BreakoutRoomService>
    {
        public BreakoutRoomServiceManager(IComponentContext context) : base(context)
        {
        }
    }
}
