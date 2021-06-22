using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Strive.Core.Extensions;
using Strive.Core.Services.Whiteboard.Actions;
using Strive.Core.Services.Whiteboard.Requests;
using Strive.Core.Services.Whiteboard.Utilities;

namespace Strive.Core.Services.Whiteboard.UseCases
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
                    participantId =
                        WhiteboardUtils.FindParticipantIdWithLastAction(whiteboard.ParticipantStates, x => x.UndoList);
                    if (participantId == null)
                        throw WhiteboardError.UndoNotAvailable.ToException();
                }

                var (undoAction, updatedStates) =
                    WhiteboardUtils.PopParticipantUndoAction(whiteboard.ParticipantStates, participantId);

                var update = undoAction.Action.Execute(whiteboard.Canvas, _canvasActionUtils);

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
