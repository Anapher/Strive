using Strive.Core.Services;
using Strive.Core.Services.ConferenceControl;

namespace Strive.Core.IntegrationTests.Services.Base
{
    public record TestParticipantConnection(Participant Participant, string ConnectionId, ParticipantMetadata Meta);
}
