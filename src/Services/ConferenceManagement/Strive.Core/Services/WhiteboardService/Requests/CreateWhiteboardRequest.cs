using MediatR;

namespace Strive.Core.Services.WhiteboardService.Requests
{
    public record CreateWhiteboardRequest(string ConferenceId, string RoomId) : IRequest;
}
