using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using PaderConference.Core.Domain.Entities;
using PaderConference.Infrastructure.Hubs.Dto;
using PaderConference.Infrastructure.Services.Synchronization;

namespace PaderConference.Infrastructure.Services.ParticipantsList
{
    public class ParticipantsListService : ConferenceService
    {
        private readonly Conference _conference;
        private readonly IMapper _mapper;
        private readonly ISynchronizedObject<IImmutableList<ParticipantDto>> _synchronizedParticipants;

        public ParticipantsListService(Conference conference, IMapper mapper,
            ISynchronizationManager synchronizationManager)
        {
            _conference = conference;
            _mapper = mapper;

            _synchronizedParticipants =
                synchronizationManager.Register<IImmutableList<ParticipantDto>>("participants",
                    ImmutableList<ParticipantDto>.Empty);
        }

        public override ValueTask OnClientConnected(Participant participant)
        {
            return UpdateParticipantsList();
        }

        public override ValueTask OnClientDisconnected(Participant participant)
        {
            return UpdateParticipantsList();
        }

        private ValueTask UpdateParticipantsList()
        {
            var participants = _conference.Participants.Values.Select(_mapper.Map<ParticipantDto>).ToImmutableList();
            return _synchronizedParticipants.Update(participants);
        }
    }
}