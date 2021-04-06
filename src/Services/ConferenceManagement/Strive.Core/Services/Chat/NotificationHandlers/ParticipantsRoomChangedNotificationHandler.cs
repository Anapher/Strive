using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Strive.Core.Services.Chat.Channels;
using Strive.Core.Services.Chat.Requests;
using Strive.Core.Services.Rooms.Notifications;
using Strive.Core.Services.Synchronization.Requests;

namespace Strive.Core.Services.Chat.NotificationHandlers
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
                await SetParticipantTypingFalseInPreviousRoom(participant);

                if (!notification.ParticipantsLeft)
                    await _mediator.Send(new UpdateSubscriptionsRequest(participant), cancellationToken);
            }
        }

        private async ValueTask SetParticipantTypingFalseInPreviousRoom(Participant participant)
        {
            var channelSubscriptions = await FetchSubscribedChannelsOfParticipant(participant);
            foreach (var channel in channelSubscriptions.OfType<RoomChatChannel>())
            {
                await _mediator.Send(new SetParticipantTypingRequest(participant, channel, false));
            }
        }

        private async ValueTask<IEnumerable<ChatChannel>> FetchSubscribedChannelsOfParticipant(Participant participant)
        {
            var subscriptions = await _mediator.Send(new FetchParticipantSubscriptionsRequest(participant));
            return subscriptions.Where(x => x.Id == SynchronizedObjectIds.CHAT).Select(ChannelSerializer.Decode);
        }
    }
}
