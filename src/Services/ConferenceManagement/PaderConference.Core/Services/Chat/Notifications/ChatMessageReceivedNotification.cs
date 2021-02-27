using System.Collections.Generic;
using MediatR;

namespace PaderConference.Core.Services.Chat.Notifications
{
    public record ChatMessageReceivedNotification(string ConferenceId, IReadOnlyList<Participant> Participants,
        ChatMessage ChatMessage, int TotalMessagesInChannel) : INotification;
}
