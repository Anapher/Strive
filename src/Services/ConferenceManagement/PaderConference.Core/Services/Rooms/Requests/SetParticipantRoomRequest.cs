using MediatR;

namespace PaderConference.Core.Services.Rooms.Requests
{
    public record SetParticipantRoomRequest(Participant Participant, string RoomId) : IRequest<Unit>;
}
