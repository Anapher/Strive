using Moq;
using PaderConference.Core.Services;
using PaderConference.Core.Services.ConferenceControl.Gateways;

namespace PaderConference.Core.Tests._TestHelpers
{
    public class JoinedParticipantsRepositoryMock
    {
        public Mock<IJoinedParticipantsRepository> Mock { get; } = new();

        public IJoinedParticipantsRepository Object => Mock.Object;

        public void JoinParticipant(Participant participant)
        {
            Mock.Setup(x => x.IsParticipantJoined(participant)).ReturnsAsync(true);
        }

        public void RemoveParticipant(Participant participant)
        {
            Mock.Setup(x => x.IsParticipantJoined(participant)).ReturnsAsync(false);
        }
    }
}
