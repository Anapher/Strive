using MediatR;
using Strive.Core.Services.ConferenceControl;

namespace Strive.Core.Services.ParticipantsList.Requests
{
    public record FetchParticipantsMetadataRequest(Participant Participant) : IRequest<ParticipantMetadata>;
}
