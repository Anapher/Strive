using MediatR;

namespace PaderConference.Core.Services.Equipment.Requests
{
    public record FetchEquipmentTokenRequest(Participant Participant) : IRequest<string>;
}
