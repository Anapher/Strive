using System.Collections.Generic;
using MediatR;
using Strive.Core.Services.Chat.Channels;

namespace Strive.Core.Services.Chat.Notifications
{
    public record ChatMessageReceivedNotification(string ConferenceId, IReadOnlyList<Participant> Participants,
        ChatMessage ChatMessage, ChatChannel Channel, int TotalMessagesInChannel) : INotification;
}
