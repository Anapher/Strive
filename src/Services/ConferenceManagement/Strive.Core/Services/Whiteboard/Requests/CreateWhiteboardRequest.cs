using MediatR;

namespace Strive.Core.Services.Whiteboard.Requests
{
    public record CreateWhiteboardRequest(string ConferenceId, string RoomId) : IRequest;
}
