using MediatR;

namespace PaderConference.Core.Services.Rooms.Requests
{
    public record SetParticipantRoomRequest(string ConferenceId, string ParticipantId, string RoomId) : IRequest<Unit>;
}
