using Autofac;
using PaderConference.Core.Services.ParticipantsList;

namespace PaderConference.Infrastructure.Services.ParticipantsList
{
    public class ParticipantsListServiceManager : AutowiredConferenceServiceManager<ParticipantsListService>
    {
        public ParticipantsListServiceManager(IComponentContext context) : base(context)
        {
        }
    }
}