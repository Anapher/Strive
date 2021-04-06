using MediatR;

namespace Strive.Core.Services.Rooms.Requests
{
    public record SetParticipantRoomRequest(Participant Participant, string RoomId) : IRequest<Unit>;
}
