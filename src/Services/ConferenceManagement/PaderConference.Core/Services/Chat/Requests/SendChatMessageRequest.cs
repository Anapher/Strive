using MediatR;
using PaderConference.Core.Interfaces;
using PaderConference.Core.Services.Chat.Channels;

namespace PaderConference.Core.Services.Chat.Requests
{
    public record SendChatMessageRequest(Participant Participant, string Message, ChatChannel Channel,
        ChatMessageOptions Options) : IRequest<SuccessOrError<Unit>>;

    public record ChatMessageOptions
    {
        public bool IsHighlighted { get; init; }
        public bool IsAnonymous { get; init; }
    }
}
