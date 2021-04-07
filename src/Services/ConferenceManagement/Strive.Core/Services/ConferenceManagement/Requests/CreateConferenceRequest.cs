using MediatR;

namespace Strive.Core.Services.ConferenceManagement.Requests
{
    public record CreateConferenceRequest(ConferenceData Data) : IRequest<string>;
}
