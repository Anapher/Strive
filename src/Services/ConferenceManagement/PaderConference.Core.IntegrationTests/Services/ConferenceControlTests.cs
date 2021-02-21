using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Autofac;
using PaderConference.Core.Domain.Entities;
using PaderConference.Core.IntegrationTests.Services.Base;
using PaderConference.Core.Services;
using PaderConference.Core.Services.ConferenceControl;
using PaderConference.Core.Services.ConferenceControl.Notifications;
using PaderConference.Core.Services.ConferenceControl.Requests;
using Xunit;
using Xunit.Abstractions;

namespace PaderConference.Core.IntegrationTests.Services
{
    public class ConferenceControlTests : ServiceIntegrationTest
    {
        private const string ConferenceId = "123";
        private const string ConnectionId = "connectionId";
        private readonly Participant _testParticipant = new(ConferenceId, "Vincent");

        public ConferenceControlTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }

        protected override void ConfigureContainer(ContainerBuilder builder)
        {
            base.ConfigureContainer(builder);
            SetupConferenceControl(builder);

            AddConferenceRepo(builder, new Conference(ConferenceId));
        }

        protected override IEnumerable<Type> FetchServiceTypes()
        {
            return FetchTypesForSynchronizedObjects();
        }

        private JoinConferenceRequest CreateTestParticipantJoinRequest()
        {
            return new(_testParticipant, ConnectionId);
        }

        private SynchronizedConferenceInfo GetSyncObjOfTestParticipant()
        {
            var syncObj = SynchronizedObjectListener.GetSynchronizedObject<SynchronizedConferenceInfo>(_testParticipant,
                SynchronizedConferenceInfoProvider.SynchronizedObjectId);

            Assert.NotNull(syncObj);
            return syncObj!;
        }

        [Fact]
        public async Task OpenConference_ConferenceNotOpen_PublishNotification()
        {
            // act
            await Mediator.Send(new OpenConferenceRequest(ConferenceId));

            // assert
            NotificationCollector.AssertSingleNotificationIssued<ConferenceOpenedNotification>();
        }

        [Fact]
        public async Task OpenConference_ConferenceAlreadyOpen_DontPublishNotification()
        {
            // arrange
            await Mediator.Send(new OpenConferenceRequest(ConferenceId));
            NotificationCollector.Reset();

            // act
            await Mediator.Send(new OpenConferenceRequest(ConferenceId));

            // assert
            NotificationCollector.AssertNoMoreNotifications();
        }

        [Fact]
        public async Task OpenConference_ConferenceNotOpen_UpdateSyncObject()
        {
            // arrange
            await Mediator.Send(CreateTestParticipantJoinRequest());

            // act
            await Mediator.Send(new OpenConferenceRequest(ConferenceId));

            // assert
            var syncObj = GetSyncObjOfTestParticipant();
            Assert.True(syncObj.IsOpen);
        }

        [Fact]
        public async Task JoinConference_ConferenceNotOpen_SynchronizedObjectConferenceIsNotOpen()
        {
            // act
            await Mediator.Send(CreateTestParticipantJoinRequest());

            // assert
            var syncObj = GetSyncObjOfTestParticipant();
            Assert.False(syncObj.IsOpen);
        }

        [Fact]
        public async Task CloseConference_NotOpen_DontPublishNotification()
        {
            // act
            await Mediator.Send(new CloseConferenceRequest(ConferenceId));

            // assert
            NotificationCollector.AssertNoMoreNotifications();
        }

        [Fact]
        public async Task CloseConference_IsOpened_PublishNotification()
        {
            // arrange
            await Mediator.Send(new OpenConferenceRequest(ConferenceId));
            NotificationCollector.Reset();

            // act
            await Mediator.Send(new CloseConferenceRequest(ConferenceId));

            // assert
            NotificationCollector.AssertSingleNotificationIssued<ConferenceClosedNotification>();
        }

        [Fact]
        public async Task CloseConference_IsOpened_UpdateSynchronizedObject()
        {
            // arrange
            await Mediator.Send(new OpenConferenceRequest(ConferenceId));
            await Mediator.Send(CreateTestParticipantJoinRequest());

            // act
            await Mediator.Send(new CloseConferenceRequest(ConferenceId));

            // assert
            var syncObj = GetSyncObjOfTestParticipant();
            Assert.False(syncObj.IsOpen);
        }

        [Fact]
        public async Task JoinConference_ParticipantAlreadyJoined_KickOldParticipant()
        {
            const string oldConnectionId = "oldConnectionId";

            // arrange
            await Mediator.Send(new JoinConferenceRequest(_testParticipant, oldConnectionId));

            // act
            await Mediator.Send(new JoinConferenceRequest(_testParticipant, ConnectionId));

            // assert
            NotificationCollector.AssertSingleNotificationIssued<ParticipantKickedNotification>(notification =>
            {
                Assert.Equal(_testParticipant, notification.Participant);
                Assert.Equal(oldConnectionId, notification.ConnectionId);
            });
        }

        [Fact]
        public async Task KickParticipant_ValidParticipant_PublishParticipantKickedNotification()
        {
            // act
            await Mediator.Send(new KickParticipantRequest(_testParticipant));

            // assert
            NotificationCollector.AssertSingleNotificationIssued<ParticipantKickedNotification>(notification =>
            {
                Assert.Equal(_testParticipant, notification.Participant);
            });
        }
    }
}
