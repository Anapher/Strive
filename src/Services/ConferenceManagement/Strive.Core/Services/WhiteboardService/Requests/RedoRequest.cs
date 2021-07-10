using MediatR;

namespace Strive.Core.Services.WhiteboardService.Requests
{
    public record RedoRequest
        (string ConferenceId, string RoomId, string WhiteboardId, string? ParticipantId) : IRequest;
}
