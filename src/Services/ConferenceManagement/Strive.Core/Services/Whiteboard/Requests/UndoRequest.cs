using MediatR;

namespace Strive.Core.Services.Whiteboard.Requests
{
    public record UndoRequest
        (string ConferenceId, string RoomId, string WhiteboardId, string? ParticipantId) : IRequest;
}
