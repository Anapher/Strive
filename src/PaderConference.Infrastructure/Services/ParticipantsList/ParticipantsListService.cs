using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using PaderConference.Core.Domain.Entities;
using PaderConference.Core.Interfaces.Services;
using PaderConference.Infrastructure.Hubs.Dto;
using PaderConference.Infrastructure.Services.Synchronization;

namespace PaderConference.Infrastructure.Services.ParticipantsList
{
    public class ParticipantsListService : ConferenceService
    {
        private readonly string _conferenceId;
        private readonly IConferenceManager _conferenceManager;
        private readonly IMapper _mapper;
        private readonly ISynchronizedObject<IImmutableList<ParticipantDto>> _synchronizedParticipants;

        public ParticipantsListService(string conferenceId, IMapper mapper, IConferenceManager conferenceManager,
            ISynchronizationManager synchronizationManager)
        {
            _conferenceId = conferenceId;
            _mapper = mapper;
            _conferenceManager = conferenceManager;

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
            var participants = _conferenceManager.GetParticipants(_conferenceId);
            if (participants == null)
                return new ValueTask();

            return _synchronizedParticipants.Update(participants.Select(_mapper.Map<ParticipantDto>).ToImmutableList());
        }
    }
}