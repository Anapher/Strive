using Strive.Core.Domain.Entities;
using Strive.Core.Services;
using Strive.Core.Services.ConferenceControl;

namespace Strive.Core.IntegrationTests.Services.Base
{
    public static class TestData
    {
        public const string ConferenceId = "testConferenceId";

        public static readonly TestParticipantConnection Vincent = CreateTestParticipantConnection("Vincent");
        public static readonly TestParticipantConnection Sven = CreateTestParticipantConnection("Sven");

        private static TestParticipantConnection CreateTestParticipantConnection(string name)
        {
            return new(new Participant(ConferenceId, $"idOf{name}"), $"connectionIdOf{name}",
                new ParticipantMetadata(name));
        }

        public static Conference GetConference()
        {
            return new(ConferenceId);
        }
    }
}
