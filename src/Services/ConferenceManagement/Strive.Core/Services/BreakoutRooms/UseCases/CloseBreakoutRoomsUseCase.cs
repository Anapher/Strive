using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Strive.Core.Services.BreakoutRooms.Internal;
using Strive.Core.Services.BreakoutRooms.Requests;

namespace Strive.Core.Services.BreakoutRooms.UseCases
{
    public class CloseBreakoutRoomsUseCase : IRequestHandler<CloseBreakoutRoomsRequest>
    {
        private readonly IMediator _mediator;

        public CloseBreakoutRoomsUseCase(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<Unit> Handle(CloseBreakoutRoomsRequest request, CancellationToken cancellationToken)
        {
            await _mediator.Send(new ApplyBreakoutRoomRequest(request.ConferenceId, null), cancellationToken);
            return Unit.Value;
        }
    }
}
