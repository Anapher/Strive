using System.Collections.Generic;
using MediatR;
using PaderConference.Core.Services.Chat.Channels;

namespace PaderConference.Core.Services.Chat.Notifications
{
    public record ChatMessageReceivedNotification(string ConferenceId, IReadOnlyList<Participant> Participants,
        ChatMessage ChatMessage, ChatChannel Channel, int TotalMessagesInChannel) : INotification;
}
