using MediatR;

namespace PaderConference.Core.Services.Equipment.Requests
{
    public record AuthenticateEquipmentRequest(Participant Participant, string Token) : IRequest;
}
