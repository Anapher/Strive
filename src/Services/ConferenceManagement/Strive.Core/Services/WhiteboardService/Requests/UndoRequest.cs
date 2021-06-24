using MediatR;

namespace Strive.Core.Services.WhiteboardService.Requests
{
    public record UndoRequest
        (string ConferenceId, string RoomId, string WhiteboardId, string? ParticipantId) : IRequest;
}
