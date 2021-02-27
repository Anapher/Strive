using MediatR;
using PaderConference.Core.Services.ConferenceControl;

namespace PaderConference.Core.Services.ParticipantsList.Requests
{
    public record FetchParticipantsMetadataRequest(Participant Participant) : IRequest<ParticipantMetadata>;
}
