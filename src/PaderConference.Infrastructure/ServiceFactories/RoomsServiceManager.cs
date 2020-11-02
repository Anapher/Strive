using Autofac;
using PaderConference.Core.Services.Rooms;

namespace PaderConference.Infrastructure.Services.Rooms
{
    public class RoomsServiceManager : AutowiredConferenceServiceManager<RoomsService>
    {
        public RoomsServiceManager(IComponentContext context) : base(context)
        {
        }
    }
}