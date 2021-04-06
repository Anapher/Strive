using MediatR;
using Strive.Core.Interfaces;
using Strive.Core.Services.Chat.Channels;

namespace Strive.Core.Services.Chat.Requests
{
    public record SendChatMessageRequest(Participant Participant, string Message, ChatChannel Channel,
        ChatMessageOptions Options) : IRequest<SuccessOrError<Unit>>;

    public record ChatMessageOptions
    {
        public bool IsAnnouncement { get; init; }
        public bool IsAnonymous { get; init; }
    }
}
