using MediatR;
using PaderConference.Core.Services.Chat.Channels;

namespace PaderConference.Core.Services.Chat.Requests
{
    public record SetParticipantTypingRequest(Participant Participant, ChatChannel Channel, bool IsTyping) : IRequest;
}
