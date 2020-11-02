using Autofac;
using PaderConference.Core.Services.ParticipantsList;
using PaderConference.Infrastructure.ServiceFactories.Base;

namespace PaderConference.Infrastructure.ServiceFactories
{
    public class ParticipantsListServiceManager : AutowiredConferenceServiceManager<ParticipantsListService>
    {
        public ParticipantsListServiceManager(IComponentContext context) : base(context)
        {
        }
    }
}