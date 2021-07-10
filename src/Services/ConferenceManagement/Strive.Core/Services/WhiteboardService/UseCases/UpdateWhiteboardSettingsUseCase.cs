using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Strive.Core.Services.WhiteboardService.Requests;

namespace Strive.Core.Services.WhiteboardService.UseCases
{
    public class UpdateWhiteboardSettingsUseCase : IRequestHandler<UpdateWhiteboardSettingsRequest>
    {
        private readonly IMediator _mediator;

        public UpdateWhiteboardSettingsUseCase(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<Unit> Handle(UpdateWhiteboardSettingsRequest request, CancellationToken cancellationToken)
        {
            var (conferenceId, roomId, whiteboardId, settings) = request;

            Whiteboard UpdateAction(Whiteboard whiteboard)
            {
                return whiteboard with
                {
                    AnyoneCanEdit = settings.AnyoneCanEdit,
                };
            }

            await _mediator.Send(new UpdateWhiteboardRequest(conferenceId, roomId, whiteboardId, UpdateAction),
                cancellationToken);
            return Unit.Value;
        }
    }
}
