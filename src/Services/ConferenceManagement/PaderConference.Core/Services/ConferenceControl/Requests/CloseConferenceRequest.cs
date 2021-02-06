using MediatR;

namespace PaderConference.Core.Services.ConferenceControl.Requests
{
    public record CloseConferenceRequest(string ConferenceId) : IRequest;
}
