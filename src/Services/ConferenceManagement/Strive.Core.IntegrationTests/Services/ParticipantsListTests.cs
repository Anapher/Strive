using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Strive.Core.Domain.Entities;
using Strive.Core.IntegrationTests.Services.Base;
using Strive.Core.Notifications;
using Strive.Core.Services;
using Strive.Core.Services.ParticipantsList;
using Strive.Core.Services.ParticipantsList.Requests;
using Xunit;
using Xunit.Abstractions;

namespace Strive.Core.IntegrationTests.Services
{
    public class ParticipantsListTests : ServiceIntegrationTest
    {
        private readonly Conference _conference = TestData.GetConference();

        public ParticipantsListTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }

        protected override void ConfigureContainer(ContainerBuilder builder)
        {
            base.ConfigureContainer(builder);

            AddConferenceRepo(builder, TestData.ConferenceId, () => _conference);
            SetupConferenceControl(builder);
        }

        protected override IEnumerable<Type> FetchServiceTypes()
        {
            return FetchTypesOfNamespace(typeof(SynchronizedParticipants)).Concat(FetchTypesForSynchronizedObjects());
        }

        private SynchronizedParticipants GetSyncObj(Participant participant)
        {
            return SynchronizedObjectListener.GetSynchronizedObject<SynchronizedParticipants>(participant,
                SynchronizedParticipants.SyncObjId);
        }

        private void AddParticipantToConferenceModerators(Participant participant)
        {
            _conference.Configuration.Moderators = _conference.Configuration.Moderators.Add(participant.Id);
        }

        [Fact]
        public async Task ParticipantJoined_GetSynchronizedObject_ParticipantIsInList()
        {
            var testParticipantConnection = TestData.Vincent;
            var (participant, _, meta) = testParticipantConnection;

            // act
            await JoinParticipant(testParticipantConnection);

            // assert
            var syncObj = GetSyncObj(participant);
            var (participantId, data) = Assert.Single(syncObj.Participants);
            Assert.Equal(participant.Id, participantId);
            Assert.Equal(meta.DisplayName, data.DisplayName);
            Assert.False(data.IsModerator);
        }

        [Fact]
        public async Task ParticipantJoined_ParticipantIsModerator_SyncObjIsModeratorIsTrue()
        {
            var testParticipant = TestData.Vincent;
            AddParticipantToConferenceModerators(testParticipant.Participant);

            // act
            await JoinParticipant(testParticipant);

            // assert
            var syncObj = GetSyncObj(testParticipant.Participant);
            var (_, data) = Assert.Single(syncObj.Participants);
            Assert.True(data.IsModerator);
        }

        [Fact]
        public async Task ParticipantJoined_ParticipantAlreadyJoined_UpdateParticipantsForAlreadyJoined()
        {
            await JoinParticipant(TestData.Vincent);

            // act
            await JoinParticipant(TestData.Sven);

            // assert
            var syncObj = GetSyncObj(TestData.Vincent.Participant);
            Assert.Equal(2, syncObj.Participants.Count);
        }

        [Fact]
        public async Task ParticipantLeft_TwoParticipantsJoined_UpdateSyncObj()
        {
            await JoinParticipant(TestData.Vincent);
            await JoinParticipant(TestData.Sven);

            // act
            await NotifyParticipantLeft(TestData.Sven);

            // assert
            var syncObj = GetSyncObj(TestData.Vincent.Participant);
            Assert.Single(syncObj.Participants);
        }

        [Fact]
        public async Task ConferenceUpdated_ParticipantAddedToConferenceModerators_UpdateSyncObj()
        {
            var testParticipant = TestData.Vincent;
            await JoinParticipant(testParticipant);

            // act
            AddParticipantToConferenceModerators(testParticipant.Participant);
            await Mediator.Publish(new ConferenceUpdatedNotification(_conference));

            // assert
            var syncObj = GetSyncObj(testParticipant.Participant);
            var (_, data) = Assert.Single(syncObj.Participants);
            Assert.True(data.IsModerator);
        }

        [Fact]
        public async Task FetchParticipantMetadata_ParticipantNotJoined_ThrowException()
        {
            // act
            await Assert.ThrowsAsync<ParticipantNotFoundException>(async () =>
                await Mediator.Send(new FetchParticipantsMetadataRequest(TestData.Vincent.Participant)));
        }

        [Fact]
        public async Task FetchParticipantMetadata_ParticipantJoined_ReturnData()
        {
            // arrange
            var testParticipant = TestData.Vincent;
            await JoinParticipant(testParticipant);

            // act
            var result = await Mediator.Send(new FetchParticipantsMetadataRequest(TestData.Vincent.Participant));

            // assert
            Assert.Equal(testParticipant.Meta, result);
        }
    }
}
