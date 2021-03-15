using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PaderConference.Core.Interfaces.Services;
using PaderConference.Core.Services.BreakoutRooms.Gateways;
using PaderConference.Core.Services.BreakoutRooms.Notifications;
using PaderConference.Core.Services.ConferenceControl.Notifications;

namespace PaderConference.Core.Services.BreakoutRooms.NotificationHandlers
{
    public class ConferenceClosedNotificationHandler : INotificationHandler<ConferenceClosedNotification>
    {
        private readonly IBreakoutRoomRepository _repository;
        private readonly IScheduledMediator _scheduledMediator;

        public ConferenceClosedNotificationHandler(IBreakoutRoomRepository repository,
            IScheduledMediator scheduledMediator)
        {
            _repository = repository;
            _scheduledMediator = scheduledMediator;
        }

        public async Task Handle(ConferenceClosedNotification notification, CancellationToken cancellationToken)
        {
            var currentState = await _repository.Get(notification.ConferenceId);
            if (currentState?.TimerTokenId != null)
                await _scheduledMediator.Remove<BreakoutRoomTimerElapsedNotification>(currentState.TimerTokenId);
        }
    }
}
