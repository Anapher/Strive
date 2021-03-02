using MediatR;
using PaderConference.Core.Dto.Services;

namespace PaderConference.Core.Services.ConferenceManagement.Requests
{
    public record CreateConferenceRequest(ConferenceData Data) : IRequest<string>;
}
