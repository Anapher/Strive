using MediatR;

namespace Strive.Core.Services.Equipment.Requests
{
    public record AuthenticateEquipmentRequest(Participant Participant, string Token) : IRequest;
}
