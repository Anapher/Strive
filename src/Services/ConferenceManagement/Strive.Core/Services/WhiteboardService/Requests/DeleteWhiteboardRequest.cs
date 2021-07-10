using MediatR;

namespace Strive.Core.Services.WhiteboardService.Requests
{
    public record DeleteWhiteboardRequest(string ConferenceId, string RoomId, string WhiteboardId) : IRequest;
}
