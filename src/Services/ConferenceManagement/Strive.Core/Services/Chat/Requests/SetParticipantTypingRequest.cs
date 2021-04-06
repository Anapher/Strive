using MediatR;
using Strive.Core.Services.Chat.Channels;

namespace Strive.Core.Services.Chat.Requests
{
    public record SetParticipantTypingRequest(Participant Participant, ChatChannel Channel, bool IsTyping) : IRequest;
}
