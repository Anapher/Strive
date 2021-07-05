using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Strive.Core.Services.Rooms;
using Strive.Core.Services.Synchronization.Extensions;
using Strive.Core.Services.WhiteboardService.Notifications;
using Strive.Core.Services.WhiteboardService.Requests;

namespace Strive.Core.Services.WhiteboardService.UseCases
{
    public class PushLiveActionUseCase : IRequestHandler<PushLiveActionRequest, Unit>
    {
        private readonly IMediator _mediator;

        public PushLiveActionUseCase(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<Unit> Handle(PushLiveActionRequest request, CancellationToken cancellationToken)
        {
            var rooms = await _mediator.FetchSynchronizedObject<SynchronizedRooms>(request.ConferenceId,
                SynchronizedRooms.SyncObjId);

            var participants = rooms.Participants.Where(x => x.Value == request.RoomId)
                .Select(x => new Participant(request.ConferenceId, x.Key));

            await _mediator.Publish(new LiveActionPushedNotification(participants.ToList(), request.ParticipantId,
                request.WhiteboardId, request.Action));

            return Unit.Value;
        }
    }
}
