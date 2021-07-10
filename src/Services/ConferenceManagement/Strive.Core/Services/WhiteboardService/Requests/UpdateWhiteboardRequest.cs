using System;
using MediatR;
using Strive.Core.Services.WhiteboardService.Responses;

namespace Strive.Core.Services.WhiteboardService.Requests
{
    public record UpdateWhiteboardRequest(string ConferenceId, string RoomId, string WhiteboardId,
        Func<Whiteboard, Whiteboard> Action) : IRequest<WhiteboardUpdatedResponse>;
}
