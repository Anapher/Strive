using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PaderConference.Core.Services.Chat.Channels;
using PaderConference.Core.Services.Chat.Gateways;
using PaderConference.Core.Services.Rooms.Notifications;

namespace PaderConference.Core.Services.Chat.NotificationHandlers
{
    public class RoomsRemovedNotificationHandler : INotificationHandler<RoomsRemovedNotification>
    {
        private readonly IChatRepository _chatRepository;

        public RoomsRemovedNotificationHandler(IChatRepository chatRepository)
        {
            _chatRepository = chatRepository;
        }

        public async Task Handle(RoomsRemovedNotification notification, CancellationToken cancellationToken)
        {
            foreach (var removedRoomId in notification.RemovedRoomIds)
            {
                var channel = new RoomChatChannel(removedRoomId);
                var channelId = ChannelSerializer.Encode(channel);
                await _chatRepository.DeleteChannel(notification.ConferenceId, channelId.ToString());
            }
        }
    }
}
