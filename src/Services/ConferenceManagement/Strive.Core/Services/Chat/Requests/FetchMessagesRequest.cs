using MediatR;
using Strive.Core.Interfaces.Gateways;
using Strive.Core.Services.Chat.Channels;

namespace Strive.Core.Services.Chat.Requests
{
    public record FetchMessagesRequest
        (string ConferenceId, ChatChannel Channel, int Start, int End) : IRequest<EntityPage<ChatMessage>>;
}
