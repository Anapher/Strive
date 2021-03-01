using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PaderConference.Core.Services.Rooms.Notifications;
using PaderConference.Core.Services.Synchronization.Requests;

namespace PaderConference.Core.Services.Chat.NotificationHandlers
{
    public class ParticipantsRoomChangedNotificationHandler : INotificationHandler<ParticipantsRoomChangedNotification>
    {
        private readonly IMediator _mediator;

        public ParticipantsRoomChangedNotificationHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task Handle(ParticipantsRoomChangedNotification notification, CancellationToken cancellationToken)
        {
            foreach (var participant in notification.Participants)
            {
                await _mediator.Send(new UpdateSubscriptionsRequest(participant), cancellationToken);
            }
        }
    }
}
