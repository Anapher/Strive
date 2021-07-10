using MediatR;

namespace Strive.Core.Services.WhiteboardService.Requests
{
    public record UpdateWhiteboardSettingsRequest (string ConferenceId, string RoomId, string WhiteboardId,
        WhiteboardSettings NewValue) : IRequest<Unit>;
}
