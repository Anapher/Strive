using Moq;
using PaderConference.Core.Services.ConferenceControl.Gateways;

namespace PaderConference.Core.Tests._TestHelpers
{
    public class JoinedParticipantsRepositoryMock
    {
        public Mock<IJoinedParticipantsRepository> Mock { get; } = new();

        public IJoinedParticipantsRepository Object => Mock.Object;

        public void JoinParticipant(string conferenceId, string participantId)
        {
            Mock.Setup(x => x.IsParticipantJoined(conferenceId, participantId)).ReturnsAsync(true);
        }

        public void RemoveParticipant(string conferenceId, string participantId)
        {
            Mock.Setup(x => x.IsParticipantJoined(conferenceId, participantId)).ReturnsAsync(false);
        }
    }
}
