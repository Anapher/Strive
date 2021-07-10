using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Strive.Core.Extensions;
using Strive.Core.Services.Synchronization.Requests;
using Strive.Core.Services.WhiteboardService.Gateways;
using Strive.Core.Services.WhiteboardService.Requests;

namespace Strive.Core.Services.WhiteboardService.UseCases
{
    public class DeleteWhiteboardUseCase : IRequestHandler<DeleteWhiteboardRequest>
    {
        private readonly IMediator _mediator;
        private readonly IWhiteboardRepository _repository;

        public DeleteWhiteboardUseCase(IMediator mediator, IWhiteboardRepository repository)
        {
            _mediator = mediator;
            _repository = repository;
        }

        public async Task<Unit> Handle(DeleteWhiteboardRequest request, CancellationToken cancellationToken)
        {
            var (conferenceId, roomId, whiteboardId) = request;

            await using (await _repository.LockWhiteboard(conferenceId, roomId, whiteboardId))
            {
                var whiteboard = await _repository.Get(conferenceId, roomId, whiteboardId);
                if (whiteboard == null)
                    throw WhiteboardError.WhiteboardNotFound.ToException();

                await _repository.Delete(conferenceId, roomId, whiteboardId);
            }

            await _mediator.Send(new UpdateSynchronizedObjectRequest(conferenceId,
                SynchronizedWhiteboards.SyncObjId(roomId)));

            return Unit.Value;
        }
    }
}
