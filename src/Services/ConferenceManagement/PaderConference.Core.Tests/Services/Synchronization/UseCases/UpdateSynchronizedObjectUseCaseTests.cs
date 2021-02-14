using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using MediatR;
using Moq;
using PaderConference.Core.Services.Synchronization;
using PaderConference.Core.Services.Synchronization.Gateways;
using PaderConference.Core.Services.Synchronization.Notifications;
using PaderConference.Core.Services.Synchronization.Requests;
using PaderConference.Core.Services.Synchronization.UseCases;
using PaderConference.Core.Tests._TestUtils;
using Xunit;

namespace PaderConference.Core.Tests.Services.Synchronization.UseCases
{
    public class UpdateSynchronizedObjectUseCaseTests
    {
        private readonly ContainerBuilder _builder = new();
        private readonly Mock<ISynchronizedObjectSubscriptionsRepository> _subscriptionsRepo = new();
        private readonly Mock<ISynchronizedObjectRepository> _syncObjRepo = new();
        private readonly Mock<IMediator> _mediator = new();

        public UpdateSynchronizedObjectUseCase Create()
        {
            return new(_builder.Build(), _syncObjRepo.Object, _subscriptionsRepo.Object, _mediator.Object);
        }

        private void AddResultForParticipant(Mock<ISynchronizedObjectProvider> mock, string conferenceId,
            string participantId, string syncObjId, object value)
        {
            mock.Setup(x => x.CanSubscribe(conferenceId, participantId))
                .ThrowsAsync(new Exception("That should not be called"));
            mock.Setup(x => x.FetchValue(conferenceId, participantId)).ReturnsAsync(value);
            mock.Setup(x => x.GetSynchronizedObjectId(conferenceId, participantId)).ReturnsAsync(syncObjId);
        }

        private void ParticipantHasSubscribed(string conferenceId, string participantId,
            IReadOnlyList<string> subscribed)
        {
            _subscriptionsRepo.Setup(x => x.Get(conferenceId, participantId)).ReturnsAsync(subscribed);
        }

        [Fact]
        public async Task Handle_SingleParticipantHasSubscribed_PublishSynchronizedObjectUpdatedNotification()
        {
            const string syncObjId = "testSyncId";
            const string conferenceId = "conid";
            const string participantId = "34";
            var syncObjValue = "testValue";

            // arrange
            var providerMock = new Mock<ISynchronizedObjectProvider>();
            AddResultForParticipant(providerMock, conferenceId, participantId, syncObjId, syncObjValue);
            _builder.RegisterInstance(providerMock.Object).AsSelf();

            ParticipantHasSubscribed(conferenceId, participantId, new[] {syncObjId});

            var capturedNotification = _mediator.CaptureNotification<SynchronizedObjectUpdatedNotification>();

            var useCase = Create();

            // act
            var request =
                new UpdateSynchronizedObjectRequest(conferenceId, new[] {participantId}, providerMock.Object.GetType());
            await useCase.Handle(request, CancellationToken.None);

            // assert
            capturedNotification.AssertReceived();

            var notification = capturedNotification.GetNotification();
            Assert.Equal(conferenceId, notification.ConferenceId);
            Assert.Equal(participantId, Assert.Single(notification.ParticipantIds));
            Assert.Equal(syncObjId, notification.SyncObjId);
            Assert.Equal(syncObjValue, notification.Value);
        }

        [Fact]
        public async Task Handle_SingleParticipantHasNotSubscribed_DoNothing()
        {
            const string syncObjId = "testSyncId";
            const string conferenceId = "conid";
            const string participantId = "34";
            var syncObjValue = "testValue";

            // arrange
            var providerMock = new Mock<ISynchronizedObjectProvider>();
            AddResultForParticipant(providerMock, conferenceId, participantId, syncObjId, syncObjValue);
            _builder.RegisterInstance(providerMock.Object).AsSelf();

            var useCase = Create();

            // act
            var request =
                new UpdateSynchronizedObjectRequest(conferenceId, new[] {participantId}, providerMock.Object.GetType());
            await useCase.Handle(request, CancellationToken.None);

            // assert
            _mediator.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task Handle_MultipleParticipantsSameSyncObject_PublishSingleSynchronizedObjectUpdatedNotification()
        {
            const string syncObjId = "testSyncId";
            const string conferenceId = "conid";
            const string participantId = "34";
            const string participantId2 = "45";
            var syncObjValue = "testValue";

            // arrange
            var providerMock = new Mock<ISynchronizedObjectProvider>();
            AddResultForParticipant(providerMock, conferenceId, participantId, syncObjId, syncObjValue);
            AddResultForParticipant(providerMock, conferenceId, participantId2, syncObjId, syncObjValue);
            _builder.RegisterInstance(providerMock.Object).AsSelf();

            ParticipantHasSubscribed(conferenceId, participantId, new[] {syncObjId});
            ParticipantHasSubscribed(conferenceId, participantId2, new[] {syncObjId});

            var capturedNotification = _mediator.CaptureNotification<SynchronizedObjectUpdatedNotification>();

            var useCase = Create();

            // act
            var request = new UpdateSynchronizedObjectRequest(conferenceId, new[] {participantId, participantId2},
                providerMock.Object.GetType());
            await useCase.Handle(request, CancellationToken.None);

            // assert
            capturedNotification.AssertReceived();

            var notification = capturedNotification.GetNotification();
            AssertHelper.AssertScrambledEquals(new[] {participantId, participantId2}, notification.ParticipantIds);
        }

        [Fact]
        public async Task
            Handle_MultipleParticipantsSameSyncObjectOnlyOneSubcribed_PublishNotificationForOneParticipant()
        {
            const string syncObjId = "testSyncId";
            const string conferenceId = "conid";
            const string participantId = "34";
            const string participantId2 = "45";
            var syncObjValue = "testValue";

            // arrange
            var providerMock = new Mock<ISynchronizedObjectProvider>();
            AddResultForParticipant(providerMock, conferenceId, participantId, syncObjId, syncObjValue);
            AddResultForParticipant(providerMock, conferenceId, participantId2, syncObjId, syncObjValue);
            _builder.RegisterInstance(providerMock.Object).AsSelf();

            ParticipantHasSubscribed(conferenceId, participantId, new[] {syncObjId});

            var capturedNotification = _mediator.CaptureNotification<SynchronizedObjectUpdatedNotification>();

            var useCase = Create();

            // act
            var request = new UpdateSynchronizedObjectRequest(conferenceId, new[] {participantId, participantId2},
                providerMock.Object.GetType());
            await useCase.Handle(request, CancellationToken.None);

            // assert
            capturedNotification.AssertReceived();

            var notification = capturedNotification.GetNotification();
            AssertHelper.AssertScrambledEquals(new[] {participantId}, notification.ParticipantIds);
        }
    }
}
