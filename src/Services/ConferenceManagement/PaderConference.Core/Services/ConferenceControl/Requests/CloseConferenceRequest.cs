using MediatR;
using PaderConference.Core.Interfaces;

namespace PaderConference.Core.Services.ConferenceControl.Requests
{
    public class CloseConferenceRequest : IRequest<SuccessOrError>
    {
        public CloseConferenceRequest(ConferenceRequestMetadata meta)
        {
            Meta = meta;
        }

        public ConferenceRequestMetadata Meta { get; }
    }
}
