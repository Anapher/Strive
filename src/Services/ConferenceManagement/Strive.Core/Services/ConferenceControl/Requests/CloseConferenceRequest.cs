using MediatR;

namespace Strive.Core.Services.ConferenceControl.Requests
{
    public record CloseConferenceRequest(string ConferenceId) : IRequest;
}
