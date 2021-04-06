using MediatR;
using Strive.Core.Dto.Services;

namespace Strive.Core.Services.ConferenceManagement.Requests
{
    public record CreateConferenceRequest(ConferenceData Data) : IRequest<string>;
}
