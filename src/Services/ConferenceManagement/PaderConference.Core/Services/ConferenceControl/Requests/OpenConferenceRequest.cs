using MediatR;

namespace PaderConference.Core.Services.ConferenceControl.Requests
{
    public record OpenConferenceRequest(string ConferenceId) : IRequest;
}
