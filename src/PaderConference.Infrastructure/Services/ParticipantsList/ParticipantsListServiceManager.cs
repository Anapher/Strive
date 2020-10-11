using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using PaderConference.Core.Domain.Entities;
using PaderConference.Infrastructure.Services.Synchronization;

namespace PaderConference.Infrastructure.Services.ParticipantsList
{
    public class ParticipantsListServiceManager : ConferenceServiceManager<ParticipantsListService>
    {
        private readonly IMapper _mapper;

        public ParticipantsListServiceManager(IMapper mapper)
        {
            _mapper = mapper;
        }

        protected override async ValueTask<ParticipantsListService> ServiceFactory(Conference conference,
            IEnumerable<IConferenceServiceManager> services)
        {
            var synchronizeService = await services.OfType<IConferenceServiceManager<SynchronizationService>>().First()
                .GetService(conference, services);

            return new ParticipantsListService(conference, _mapper,
                synchronizeService);
        }
    }
}