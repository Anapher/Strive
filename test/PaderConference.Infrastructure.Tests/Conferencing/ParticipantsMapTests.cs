using PaderConference.Core.Domain.Entities;
using PaderConference.Infrastructure.Conferencing;
using Xunit;

namespace PaderConference.Infrastructure.Tests.Conferencing
{
    public class ParticipantsMapTests
    {
        [Fact]
        public void AddParticipant_AddNew_SucceedAndAddToBothDictionaries()
        {
            const string conferenceId = "123";
            var participant = new Participant("532", "Vincent", "mod", default);

            // arrange
            var map = new ParticipantsMap();

            // act
            var result = map.AddParticipant(conferenceId, participant);

            // assert
            Assert.True(result);

            var (key, value) = Assert.Single(map.ParticipantToConference);
            Assert.Equal(participant.ParticipantId, key);
            Assert.Equal(conferenceId, value);

            var conferenceParticipants = Assert.Single(map.ConferenceParticipants);
            Assert.Equal(conferenceId, conferenceParticipants.Key);

            var mappedParticipant = Assert.Single(conferenceParticipants.Value);
            Assert.Equal(participant.ParticipantId, mappedParticipant.Key);
            Assert.Equal(participant, mappedParticipant.Value);
        }

        [Fact]
        public void AddParticipant_AddDuplicate_ReturnFalseForSecondAdd()
        {
            const string conferenceId = "123";
            var participant = new Participant("532", "Vincent", "mod", default);

            // arrange
            var map = new ParticipantsMap();

            // act
            var result = map.AddParticipant(conferenceId, participant);
            var result2 = map.AddParticipant(conferenceId, participant);

            // assert
            Assert.True(result);
            Assert.False(result2);

            Assert.Single(map.ParticipantToConference);
            var conferenceMap = Assert.Single(map.ConferenceParticipants);
            Assert.Single(conferenceMap.Value);
        }

        [Fact]
        public void RemoveParticipant_ParticipantWasNotAdded_NothingHappens()
        {
            // arrange
            var map = new ParticipantsMap();

            // act
            map.RemoveParticipant("123");

            // assert
            Assert.Empty(map.ParticipantToConference);
            Assert.Empty(map.ConferenceParticipants);
        }

        [Fact]
        public void RemoveParticipant_ParticipantExisted_Succeed()
        {
            const string conferenceId = "123";
            var participant = new Participant("532", "Vincent", "mod", default);

            // arrange
            var map = new ParticipantsMap();

            // act
            map.AddParticipant(conferenceId, participant);
            map.RemoveParticipant(participant.ParticipantId);

            // assert
            Assert.Empty(map.ParticipantToConference);
            var conferenceMap = Assert.Single(map.ConferenceParticipants);
            Assert.Empty(conferenceMap.Value);
        }
    }
}
