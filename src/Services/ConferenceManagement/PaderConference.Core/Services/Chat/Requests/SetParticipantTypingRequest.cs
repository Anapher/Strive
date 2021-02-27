using MediatR;

namespace PaderConference.Core.Services.Chat.Requests
{
    public record SetParticipantTypingRequest(Participant Participant, string Channel, bool IsTyping) : IRequest;
}
