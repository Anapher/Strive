using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Strive.Core.Extensions;
using Strive.Core.Services.Whiteboard.Actions;
using Strive.Core.Services.Whiteboard.Requests;
using Strive.Core.Services.Whiteboard.Utilities;

namespace Strive.Core.Services.Whiteboard.UseCases
{
    public class RedoUseCase : IRequestHandler<UndoRequest>
    {
        private readonly IMediator _mediator;
        private readonly ICanvasActionUtils _canvasActionUtils;

        public RedoUseCase(IMediator mediator, ICanvasActionUtils canvasActionUtils)
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
                        WhiteboardUtils.FindParticipantIdWithLastAction(whiteboard.ParticipantStates, x => x.RedoList);
                    if (participantId == null)
                        throw WhiteboardError.RedoNotAvailable.ToException();
                }

                var (redoAction, updatedStates) =
                    WhiteboardUtils.PopParticipantRedoAction(whiteboard.ParticipantStates, participantId);

                var update = redoAction.Action.Execute(whiteboard.Canvas, _canvasActionUtils);

                return whiteboard with
                {
                    Canvas = update.Canvas,
                    ParticipantStates = WhiteboardUtils.AddParticipantUndoAction(updatedStates,
                        new VersionedAction(update.UndoAction, whiteboard.Version)),
                };
            }

            return _mediator.Send(new UpdateWhiteboardRequest(conferenceId, roomId, whiteboardId, UpdateAction),
                cancellationToken);
        }
    }
}
