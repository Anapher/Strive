using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PaderConference.Core.Services.Chat.Channels;
using PaderConference.Core.Services.Chat.Requests;
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
