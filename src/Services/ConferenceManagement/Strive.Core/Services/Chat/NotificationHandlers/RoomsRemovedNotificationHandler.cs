using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Strive.Core.Services.Chat.Channels;
using Strive.Core.Services.Chat.Gateways;
using Strive.Core.Services.Rooms.Notifications;

namespace Strive.Core.Services.Chat.NotificationHandlers
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
