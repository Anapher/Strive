using System.Threading.Tasks;
using AutoMapper;
using PaderConference.Core.Interfaces.Services;
using PaderConference.Infrastructure.Services.Synchronization;

namespace PaderConference.Infrastructure.Services.ParticipantsList
{
    public class ParticipantsListServiceManager : ConferenceServiceManager<ParticipantsListService>
    {
        private readonly IConferenceManager _conferenceManager;
        private readonly IConferenceServiceManager<SynchronizationService> _synchronizationServiceManager;
        private readonly IMapper _mapper;

        public ParticipantsListServiceManager(IMapper mapper, IConferenceManager conferenceManager,
            IConferenceServiceManager<SynchronizationService> synchronizationServiceManager)
        {
            _mapper = mapper;
            _conferenceManager = conferenceManager;
            _synchronizationServiceManager = synchronizationServiceManager;
        }

        protected override async ValueTask<ParticipantsListService> ServiceFactory(string conferenceId)
        {
            var synchronizeService = await _synchronizationServiceManager.GetService(conferenceId);

            return new ParticipantsListService(conferenceId, _mapper, _conferenceManager,
                synchronizeService);
        }
    }
}