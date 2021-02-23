using PaderConference.Core.Services;
using PaderConference.Core.Services.ConferenceControl;

namespace PaderConference.Core.IntegrationTests.Services.Base
{
    public record TestParticipantConnection(Participant Participant, string ConnectionId, ParticipantMetadata Meta);
}
