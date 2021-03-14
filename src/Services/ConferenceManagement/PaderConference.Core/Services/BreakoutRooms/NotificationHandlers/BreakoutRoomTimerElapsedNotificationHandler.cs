using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PaderConference.Core.Services.BreakoutRooms.Gateways;
using PaderConference.Core.Services.BreakoutRooms.Internal;
using PaderConference.Core.Services.BreakoutRooms.Notifications;

namespace PaderConference.Core.Services.BreakoutRooms.NotificationHandlers
{
    public class
        BreakoutRoomTimerElapsedNotificationHandler : INotificationHandler<BreakoutRoomTimerElapsedNotification>
    {
        private readonly IMediator _mediator;
        private readonly IBreakoutRoomRepository _repository;

        public BreakoutRoomTimerElapsedNotificationHandler(IMediator mediator, IBreakoutRoomRepository repository)
        {
            _mediator = mediator;
            _repository = repository;
        }

        public async Task Handle(BreakoutRoomTimerElapsedNotification notification, CancellationToken cancellationToken)
        {
            await using var @lock = await _repository.LockBreakoutRooms(notification.ConferenceId);

            var currentState = await _repository.Get(notification.ConferenceId);
            if (currentState == null) return;

            if (notification.TokenId != null && currentState.TimerTokenId != notification.TokenId) return;

            await _mediator.Send(new ApplyBreakoutRoomRequest(notification.ConferenceId, null, @lock),
                cancellationToken);
        }
    }
}
