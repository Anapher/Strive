using MediatR;

namespace Strive.Core.Services.ConferenceControl.Requests
{
    public record OpenConferenceRequest(string ConferenceId) : IRequest;
}
