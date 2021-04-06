using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Strive.Core.Services.Chat;
using Strive.Core.Services.Chat.Channels;
using Strive.Core.Services.Chat.Notifications;
using Strive.Hubs.Core.Responses;

namespace Strive.Hubs.Core.NotificationHandlers
{
    public class ChatMessageReceivedNotificationHandler : INotificationHandler<ChatMessageReceivedNotification>
    {
        private readonly IHubContext<CoreHub> _hubContext;

        public ChatMessageReceivedNotificationHandler(IHubContext<CoreHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task Handle(ChatMessageReceivedNotification notification, CancellationToken cancellationToken)
        {
            var (_, participants, message, channel, id) = notification;

            ChatMessageSender? sender = null;
            if (!message.Options.IsAnonymous)
                sender = message.Sender;

            var channelId = ChannelSerializer.Encode(channel).ToString();

            var messageDto = new ChatMessageDto(id, channelId, sender, message.Message, message.Timestamp,
                message.Options);

            var participantGroups = participants.Select(CoreHubGroups.OfParticipant);
            await _hubContext.Clients.Groups(participantGroups).ChatMessage(messageDto, cancellationToken);
        }
    }
}
