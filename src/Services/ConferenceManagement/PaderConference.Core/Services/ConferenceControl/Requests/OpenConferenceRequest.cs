using MediatR;
using PaderConference.Core.Interfaces;

namespace PaderConference.Core.Services.ConferenceControl.Requests
{
    public class OpenConferenceRequest : IRequest<SuccessOrError>
    {
        public OpenConferenceRequest(ConferenceRequestMetadata meta)
        {
            Meta = meta;
        }

        public ConferenceRequestMetadata Meta { get; }
    }
}
