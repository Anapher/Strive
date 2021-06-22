using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Strive.Core.Services.Whiteboard.Actions;
using Strive.Core.Services.Whiteboard.Requests;
using Strive.Core.Services.Whiteboard.Utilities;

namespace Strive.Core.Services.Whiteboard.UseCases
{
    public class PushActionUseCase : IRequestHandler<PushActionRequest>
    {
        private readonly IMediator _mediator;
        private readonly ICanvasActionUtils _canvasActionUtils;

        public PushActionUseCase(IMediator mediator, ICanvasActionUtils canvasActionUtils)
        {
            _mediator = mediator;
            _canvasActionUtils = canvasActionUtils;
        }

        public Task<Unit> Handle(PushActionRequest request, CancellationToken cancellationToken)
        {
            var (conferenceId, roomId, whiteboardId, action) = request;

            Whiteboard Commit(Whiteboard whiteboard)
            {
                var update = action.Execute(whiteboard.Canvas, _canvasActionUtils);
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
