using System.Collections.Generic;
using System.Collections.Immutable;
using MediatR;
using Strive.Core.Services.WhiteboardService;
using Strive.Core.Services.WhiteboardService.Requests;
using Strive.Tests.Utils;

namespace Strive.Core.Tests.Services.WhiteboardService
{
    public abstract class ActionRequestTestBase
    {
        protected const string ConferenceId = "123";
        protected const string RoomId = "room1";
        protected const string WhiteboardId = "DaVinci";
        protected const string ParticipantId = "1";

        protected Whiteboard Execute(CapturedRequest<UpdateWhiteboardRequest, Unit> capturedRequest,
            Whiteboard whiteboard)
        {
            var action = capturedRequest.GetRequest().Action;
            return action(whiteboard);
        }

        protected Whiteboard CreateWhiteboard(WhiteboardCanvas canvas,
            IReadOnlyDictionary<string, ParticipantWhiteboardState> states, int version)
        {
            return new(WhiteboardId, "Da Vinci", false, canvas, states.ToImmutableDictionary(), version);
        }
    }
}
