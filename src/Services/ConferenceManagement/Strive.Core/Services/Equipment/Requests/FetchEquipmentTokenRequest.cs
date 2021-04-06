using MediatR;

namespace Strive.Core.Services.Equipment.Requests
{
    public record FetchEquipmentTokenRequest(Participant Participant) : IRequest<string>;
}
