using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Autofac;
using Moq;
using PaderConference.Core.Domain.Entities;
using PaderConference.Core.IntegrationTests.Services.Base;
using PaderConference.Core.Services;
using PaderConference.Core.Services.ConferenceControl;
using PaderConference.Core.Services.ConferenceControl.Notifications;
using PaderConference.Core.Services.ConferenceControl.Requests;
using PaderConference.Core.Services.Synchronization;
using PaderConference.Core.Services.Synchronization.Gateways;
using PaderConference.Core.Services.Synchronization.Notifications;
using PaderConference.Core.Services.Synchronization.Requests;
using Xunit;
using Xunit.Abstractions;

namespace PaderConference.Core.IntegrationTests.Services
{
    public class SynchronizationTests : ServiceIntegrationTest
    {
        private const string SyncObjId = "hello";
        private const string ConferenceId = "45";
        private const string ConnectionId = "connectionId";

        private const string ParticipantIdWithPermissions = "p1";
        private const string ParticipantIdWithoutPermissions = "p2";
        private string _syncObjValue = "value";

        private static readonly Participant ParticipantWithPermissions =
            new(ConferenceId, ParticipantIdWithPermissions);

        private static readonly Participant ParticipantWithoutPermissions =
            new(ConferenceId, ParticipantIdWithoutPermissions);

        private readonly Mock<ISynchronizedObjectProvider> _providerMock = new();

        public SynchronizationTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }

        protected override IEnumerable<Type> FetchServiceTypes()
        {
            return FetchTypesOfNamespace(typeof(SynchronizedObjectId));
        }

        protected override void ConfigureContainer(ContainerBuilder builder)
        {
            base.ConfigureContainer(builder);

            var conference = new Conference(ConferenceId);
            AddConferenceRepo(builder, conference);
            SetupConferenceControl(builder, x => !x.IsAssignableTo<ISynchronizedObjectProvider>());

            _providerMock.Setup(x => x.Id).Returns(SyncObjId);
            _providerMock
                .Setup(x => x.FetchValue(It.IsAny<string>(), It.Is<SynchronizedObjectId>(s => s.Id == SyncObjId)))
                .ReturnsAsync(() => _syncObjValue);

            builder.RegisterInstance(_providerMock.Object).As<ISynchronizedObjectProvider>();
        }

        protected override Task SetupTest(ILifetimeScope container)
        {
            SetHasPermissionsForParticipant(ParticipantWithPermissions);
            SetHasNoPermissionsForParticipant(ParticipantWithoutPermissions);

            return Task.CompletedTask;
        }

        private void SetHasPermissionsForParticipant(Participant participant)
        {
            _providerMock.Setup(x => x.GetAvailableObjects(participant))
                .ReturnsAsync(new List<SynchronizedObjectId> {new(SyncObjId)});
        }

        private void SetHasNoPermissionsForParticipant(Participant participant)
        {
            _providerMock.Setup(x => x.GetAvailableObjects(participant)).ReturnsAsync(new List<SynchronizedObjectId>());
        }

        private JoinConferenceRequest CreateJoinRequestForParticipant(Participant participant)
        {
            return new(participant, ConnectionId, new ParticipantMetadata("Olaf"));
        }

        private Task JoinParticipantWithPermissions()
        {
            return Mediator.Send(CreateJoinRequestForParticipant(ParticipantWithPermissions));
        }

        private Task JoinParticipantWithoutPermissions()
        {
            return Mediator.Send(CreateJoinRequestForParticipant(ParticipantWithoutPermissions));
        }

        private async Task AssertParticipantNoSubscriptions(Participant participant)
        {
            var subscriptionRepo = Container.Resolve<ISynchronizedObjectSubscriptionsRepository>();
            var subscriptions = await subscriptionRepo.Get(participant);
            Assert.Null(subscriptions);
        }

        private async Task AssertSyncObjectCached()
        {
            var subscriptionRepo = Container.Resolve<ISynchronizedObjectRepository>();
            var obj = await subscriptionRepo.Get(ConferenceId, SyncObjId, _syncObjValue.GetType());
            Assert.NotNull(obj);
        }

        private async Task AssertSyncObjectNotCached()
        {
            var subscriptionRepo = Container.Resolve<ISynchronizedObjectRepository>();
            var obj = await subscriptionRepo.Get(ConferenceId, SyncObjId, _syncObjValue.GetType());
            Assert.Null(obj);
        }

        [Fact]
        public async Task ParticipantJoined_WithPermissions_PublishSynchronizedObjectUpdatedNotification()
        {
            // act
            await JoinParticipantWithPermissions();

            // assert
            NotificationCollector.AssertSingleNotificationIssued<SynchronizedObjectUpdatedNotification>(
                x => x.SyncObjId == SyncObjId, n =>
                {
                    Assert.Equal(_syncObjValue, n.Value);
                    Assert.Equal(ParticipantWithPermissions, Assert.Single(n.Participants));
                });

            await AssertSyncObjectCached();
        }

        [Fact]
        public async Task ParticipantJoined_WithoutPermissions_DoNothing()
        {
            // act
            await JoinParticipantWithoutPermissions();

            // assert
            var subscriptionsObj = SynchronizedSubscriptionsProvider
                .GetObjIdOfParticipant(ParticipantIdWithoutPermissions).ToString();
            NotificationCollector.AssertSingleNotificationIssued<SynchronizedObjectUpdatedNotification>(x =>
                x.SyncObjId == subscriptionsObj);

            NotificationCollector.AssertNoNotificationOfType<SynchronizedObjectUpdatedNotification>();
        }

        [Fact]
        public async Task
            UpdateSynchronizedObjectRequest_TwoParticipants_SendNewObjectOnlyToParticipantWithPermissions()
        {
            const string newValue = "newVal";

            // arrange
            await JoinParticipantWithPermissions();
            await JoinParticipantWithoutPermissions();

            NotificationCollector.Reset();
            _syncObjValue = newValue;

            // act
            var updateRequest =
                new UpdateSynchronizedObjectRequest(ConferenceId, SynchronizedObjectId.Parse(SyncObjId));
            await Mediator.Send(updateRequest);

            // assert
            NotificationCollector.AssertSingleNotificationIssued<SynchronizedObjectUpdatedNotification>(n =>
            {
                Assert.Equal(SyncObjId, n.SyncObjId);
                Assert.Equal(newValue, n.Value);
                Assert.Equal(ParticipantWithPermissions, Assert.Single(n.Participants));
            });
            NotificationCollector.AssertNoMoreNotifications();
        }

        [Fact]
        public async Task UpdateSynchronizedObjectRequest_OneParticipantWithPermissions_VerifyPreviousValue()
        {
            const string newValue = "newVal";

            // arrange
            await JoinParticipantWithPermissions();

            NotificationCollector.Reset();
            var oldValue = _syncObjValue;
            _syncObjValue = newValue;

            // act
            var updateRequest =
                new UpdateSynchronizedObjectRequest(ConferenceId, SynchronizedObjectId.Parse(SyncObjId));
            await Mediator.Send(updateRequest);

            // assert
            NotificationCollector.AssertSingleNotificationIssued<SynchronizedObjectUpdatedNotification>(n =>
            {
                Assert.Equal(oldValue, n.PreviousValue);
            });
        }

        [Fact]
        public async Task UpdateSynchronizedObjectRequest_ObjectDidNotChange_DontPublishChangedNotification()
        {
            // arrange
            await JoinParticipantWithPermissions();
            NotificationCollector.Reset();

            // act
            var updateRequest =
                new UpdateSynchronizedObjectRequest(ConferenceId, SynchronizedObjectId.Parse(SyncObjId));
            await Mediator.Send(updateRequest);

            // assert
            NotificationCollector.AssertNoMoreNotifications();
        }

        [Fact]
        public async Task ParticipantLeft_WasTheOnlyParticipant_ClearSubscriptionsAndCachedSyncObject()
        {
            const string connectionId = "connId";

            // arrange
            await JoinParticipantWithPermissions();

            // act
            await Mediator.Publish(new ParticipantLeftNotification(ParticipantWithPermissions, connectionId));

            // assert
            await AssertParticipantNoSubscriptions(ParticipantWithPermissions);
            await AssertSyncObjectNotCached();
        }

        [Fact]
        public async Task ParticipantLeft_AnotherParticipantJoined_DontClearCachedSyncObject()
        {
            var participant2WithPermissions = new Participant(ConferenceId, "234");

            // arrange
            SetHasPermissionsForParticipant(participant2WithPermissions);

            await JoinParticipantWithPermissions();
            await Mediator.Send(CreateJoinRequestForParticipant(participant2WithPermissions));

            // act
            await Mediator.Publish(new ParticipantLeftNotification(ParticipantWithPermissions, "connid"));

            // assert
            await AssertSyncObjectCached();

            var syncObjRepo = Container.Resolve<ISynchronizedObjectRepository>();
            Assert.NotNull(await syncObjRepo.Get(ConferenceId, SyncObjId, _syncObjValue.GetType()));
        }

        [Fact]
        public async Task UpdateSubscriptions_NothingChanged_DoNothing()
        {
            // arrange
            await JoinParticipantWithPermissions();
            NotificationCollector.Reset();

            // act
            await Mediator.Send(new UpdateSubscriptionsRequest(ParticipantWithPermissions));

            // assert
            NotificationCollector.AssertNoMoreNotifications();
        }

        [Fact]
        public async Task UpdateSubscriptions_SubscriptionAdded_SendCurrentSyncObjState()
        {
            var randomParticipant = new Participant(ConferenceId, "c3a993ffed6f4c229fdfdf755cb2564e");

            // arrange
            SetHasNoPermissionsForParticipant(randomParticipant);

            await Mediator.Send(CreateJoinRequestForParticipant(randomParticipant));

            SetHasPermissionsForParticipant(randomParticipant);
            NotificationCollector.Reset();

            // act
            await Mediator.Send(new UpdateSubscriptionsRequest(randomParticipant));

            // assert
            NotificationCollector.AssertSingleNotificationIssued<SynchronizedObjectUpdatedNotification>(x =>
                x.SyncObjId == SyncObjId);
        }

        [Fact]
        public async Task UpdateSubscriptions_SubscriptionRemoved_DontSendUpdatesAnymore()
        {
            var randomParticipant = new Participant(ConferenceId, "5179b30edf324d7bb1ab77248ee8af29");

            // arrange
            SetHasPermissionsForParticipant(randomParticipant);

            await Mediator.Send(CreateJoinRequestForParticipant(randomParticipant));

            SetHasNoPermissionsForParticipant(randomParticipant);
            NotificationCollector.Reset();

            _syncObjValue = "newVal";

            // act
            await Mediator.Send(new UpdateSubscriptionsRequest(randomParticipant));
            await Mediator.Send(
                new UpdateSynchronizedObjectRequest(ConferenceId, SynchronizedObjectId.Parse(SyncObjId)));

            // assert
            var subscriptionsObjId =
                SynchronizedSubscriptionsProvider.GetObjIdOfParticipant(randomParticipant.Id).ToString();

            NotificationCollector.AssertSingleNotificationIssued<ParticipantSubscriptionsUpdatedNotification>();
            NotificationCollector.AssertSingleNotificationIssued<SynchronizedObjectUpdatedNotification>(x =>
                x.SyncObjId == subscriptionsObjId);
            NotificationCollector.AssertNoMoreNotifications();
        }

        [Fact]
        public async Task UpdateSubscriptions_ParticipantLeft_NoNotifications()
        {
            var randomParticipant = new Participant(ConferenceId, "0e6f99dc9c154febb37db4cbba056f36");

            // arrange
            SetHasPermissionsForParticipant(randomParticipant);

            await Mediator.Send(CreateJoinRequestForParticipant(randomParticipant));
            await Mediator.Publish(new ParticipantLeftNotification(randomParticipant, ConnectionId));

            NotificationCollector.Reset();

            // act
            await Mediator.Send(new UpdateSubscriptionsRequest(randomParticipant));

            // assert
            await AssertParticipantNoSubscriptions(randomParticipant);
            NotificationCollector.AssertNoMoreNotifications();
        }

        [Fact]
        public async Task FetchSynchronizedObject_NotStored_FetchSyncObjectAndReturn()
        {
            // act
            var result = await Mediator.Send(
                new FetchSynchronizedObjectRequest(ConferenceId, SynchronizedObjectId.Parse(SyncObjId)));

            // assert
            Assert.Equal(_syncObjValue, result);
        }
    }
}
