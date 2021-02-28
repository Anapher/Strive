using MediatR;
using PaderConference.Core.Interfaces.Gateways;
using PaderConference.Core.Services.Chat.Channels;

namespace PaderConference.Core.Services.Chat.Requests
{
    public record FetchMessagesRequest
        (string ConferenceId, ChatChannel Channel, int Start, int End) : IRequest<EntityPage<ChatMessage>>;
}
