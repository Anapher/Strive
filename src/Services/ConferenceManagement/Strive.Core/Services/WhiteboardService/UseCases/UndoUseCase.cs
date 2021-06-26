using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Strive.Core.Extensions;
using Strive.Core.Services.WhiteboardService.Actions;
using Strive.Core.Services.WhiteboardService.Requests;
using Strive.Core.Services.WhiteboardService.Utilities;

namespace Strive.Core.Services.WhiteboardService.UseCases
{
    public class UndoUseCase : IRequestHandler<UndoRequest>
    {
        private readonly IMediator _mediator;
        private readonly ICanvasActionUtils _canvasActionUtils;

        public UndoUseCase(IMediator mediator, ICanvasActionUtils canvasActionUtils)
        {
            _mediator = mediator;
            _canvasActionUtils = canvasActionUtils;
        }

        public Task<Unit> Handle(UndoRequest request, CancellationToken cancellationToken)
        {
            var (conferenceId, roomId, whiteboardId, participantId) = request;

            Whiteboard UpdateAction(Whiteboard whiteboard)
            {
                if (participantId == null)
                {
                    // global undo
                    participantId =
                        WhiteboardUtils.FindParticipantIdWithLastAction(whiteboard.ParticipantStates, x => x.UndoList);
                    if (participantId == null)
                        throw WhiteboardError.UndoNotAvailable.ToException();
                }

                var (undoAction, updatedStates) =
                    WhiteboardUtils.PopParticipantUndoAction(whiteboard.ParticipantStates, participantId);

                var update = undoAction.Action.Execute(whiteboard.Canvas, _canvasActionUtils, whiteboard.Version + 1);
                if (update == null)
                {
                    return whiteboard with {ParticipantStates = updatedStates};
                }

                return whiteboard with
                {
                    Canvas = update.Canvas,
                    ParticipantStates = WhiteboardUtils.AddParticipantRedoAction(updatedStates,
                        new VersionedAction(update.UndoAction, whiteboard.Version)),
                };
            }

            return _mediator.Send(new UpdateWhiteboardRequest(conferenceId, roomId, whiteboardId, UpdateAction),
                cancellationToken);
        }
    }
}
