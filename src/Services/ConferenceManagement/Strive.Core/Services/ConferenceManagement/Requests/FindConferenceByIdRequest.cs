using MediatR;
using Strive.Core.Domain.Entities;

namespace Strive.Core.Services.ConferenceManagement.Requests
{
    public record FindConferenceByIdRequest(string ConferenceId) : IRequest<Conference>;
}
