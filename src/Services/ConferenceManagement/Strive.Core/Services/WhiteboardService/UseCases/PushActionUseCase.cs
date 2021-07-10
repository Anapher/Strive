using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Strive.Core.Extensions;
using Strive.Core.Services.WhiteboardService.Requests;
using Strive.Core.Services.WhiteboardService.Responses;
using Strive.Core.Services.WhiteboardService.Utilities;

namespace Strive.Core.Services.WhiteboardService.UseCases
{
    public class PushActionUseCase : IRequestHandler<PushActionRequest, WhiteboardUpdatedResponse>
    {
        private readonly ICanvasActionUtils _canvasActionUtils;
        private readonly IMediator _mediator;

        public PushActionUseCase(IMediator mediator, ICanvasActionUtils canvasActionUtils)
        {
            _mediator = mediator;
            _canvasActionUtils = canvasActionUtils;
        }

        public Task<WhiteboardUpdatedResponse> Handle(PushActionRequest request, CancellationToken cancellationToken)
        {
            var (conferenceId, roomId, whiteboardId, participantId, pushAction) = request;
            var action = pushAction.ConvertToAction(participantId);

            Whiteboard Commit(Whiteboard whiteboard)
            {
                var update = action.Execute(whiteboard.Canvas, _canvasActionUtils, whiteboard.Version + 1);
                if (update == null)
                {
                    throw WhiteboardError.WhiteboardActionHadNoEffect.ToException();
                }

                var undoAction = new VersionedAction(update.UndoAction, whiteboard.Version);
                return whiteboard with
                {
                    Canvas = update.Canvas,
                    ParticipantStates =
                    WhiteboardUtils.AddParticipantUndoActionAndClearRedo(whiteboard.ParticipantStates, undoAction),
                };
            }

            return _mediator.Send(new UpdateWhiteboardRequest(conferenceId, roomId, whiteboardId, Commit),
                cancellationToken);
        }
    }
}
