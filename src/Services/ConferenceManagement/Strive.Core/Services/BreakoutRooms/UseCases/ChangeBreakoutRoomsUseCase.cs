using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Strive.Core.Interfaces;
using Strive.Core.Services.BreakoutRooms.Gateways;
using Strive.Core.Services.BreakoutRooms.Internal;
using Strive.Core.Services.BreakoutRooms.Requests;

namespace Strive.Core.Services.BreakoutRooms.UseCases
{
    public class ChangeBreakoutRoomsUseCase : IRequestHandler<ChangeBreakoutRoomsRequest, SuccessOrError<Unit>>
    {
        private readonly IMediator _mediator;
        private readonly IBreakoutRoomRepository _repository;

        public ChangeBreakoutRoomsUseCase(IMediator mediator, IBreakoutRoomRepository repository)
        {
            _mediator = mediator;
            _repository = repository;
        }

        public async Task<SuccessOrError<Unit>> Handle(ChangeBreakoutRoomsRequest request,
            CancellationToken cancellationToken)
        {
            var (conferenceId, patch) = request;
            await using (var @lock = await _repository.LockBreakoutRooms(conferenceId))
            {
                var current = await _repository.Get(conferenceId);
                if (current == null)
                    return BreakoutRoomError.NotOpen;

                var newState = current.Config with { }; // clone
                patch.ApplyTo(newState);

                await _mediator.Send(new ApplyBreakoutRoomRequest(conferenceId, newState, @lock));
            }

            return SuccessOrError<Unit>.Succeeded(Unit.Value);
        }
    }
}
