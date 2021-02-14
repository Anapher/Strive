using System.Collections.Generic;
using System.Threading.Tasks;
using PaderConference.Core.Services.ConferenceControl.Gateways;
using PaderConference.Core.Services.Rooms.Gateways;

namespace PaderConference.Core.Services.Synchronization
{
    public class ParticipantAggregator : IParticipantAggregator
    {
        private readonly IJoinedParticipantsRepository _joinedParticipantsRepository;
        private readonly IRoomRepository _roomRepository;

        public ParticipantAggregator(IJoinedParticipantsRepository joinedParticipantsRepository,
            IRoomRepository roomRepository)
        {
            _joinedParticipantsRepository = joinedParticipantsRepository;
            _roomRepository = roomRepository;
        }

        public async ValueTask<IEnumerable<string>> OfConference(string conferenceId)
        {
            return await _joinedParticipantsRepository.GetParticipantsOfConference(conferenceId);
        }

        public async ValueTask<IEnumerable<string>> OfRoom(string conferenceId, string roomId)
        {
            return await _roomRepository.GetParticipantsOfRoom(conferenceId, roomId);
        }
    }
}
