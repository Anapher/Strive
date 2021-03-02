using MediatR;
using PaderConference.Core.Domain.Entities;

namespace PaderConference.Core.Services.ConferenceManagement.Requests
{
    public record FindConferenceByIdRequest(string ConferenceId) : IRequest<Conference>;
}
