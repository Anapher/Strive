using System.Collections.Generic;
using MediatR;
using Strive.Core.Extensions;

namespace Strive.Core.Services.Rooms.Requests
{
    public record SetParticipantRoomRequest (string ConferenceId,
        IEnumerable<(string participantId, string roomId)> RoomAssignments) : IRequest<Unit>
    {
        public static SetParticipantRoomRequest MoveParticipant(Participant participant, string roomId)
        {
            return new(participant.ConferenceId, (participant.Id, roomId).Yield());
        }
    }
}
