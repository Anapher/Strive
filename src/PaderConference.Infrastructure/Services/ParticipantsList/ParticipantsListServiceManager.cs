using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using PaderConference.Core.Interfaces.Services;
using PaderConference.Infrastructure.Services.Synchronization;

namespace PaderConference.Infrastructure.Services.ParticipantsList
{
    public class ParticipantsListServiceManager : ConferenceServiceManager<ParticipantsListService>
    {
        private readonly IConferenceManager _conferenceManager;
        private readonly IMapper _mapper;

        public ParticipantsListServiceManager(IMapper mapper, IConferenceManager conferenceManager)
        {
            _mapper = mapper;
            _conferenceManager = conferenceManager;
        }

        protected override async ValueTask<ParticipantsListService> ServiceFactory(string conferenceId,
            IEnumerable<IConferenceServiceManager> services)
        {
            var synchronizeService = await services.OfType<IConferenceServiceManager<SynchronizationService>>().First()
                .GetService(conferenceId, services);

            return new ParticipantsListService(conferenceId, _mapper, _conferenceManager,
                synchronizeService);
        }
    }
}