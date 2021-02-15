using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
        private readonly Mock<ISynchronizedObjectSubscriptionsRepository> _subscriptionsRepo = new();
        private readonly Mock<ISynchronizedObjectRepository> _syncObjRepo = new();
        private readonly Mock<IMediator> _mediator = new();
        private readonly List<ISynchronizedObjectProvider> _providers = new();
        private readonly Dictionary<string, List<string>> _participantSubscriptions = new();

        private const string ConferenceId = "123";

        public UpdateSynchronizedObjectUseCase Create()
        {
            return new(_providers, _syncObjRepo.Object, _subscriptionsRepo.Object, _mediator.Object);
        }

        private void SetupProvider(SynchronizedObjectId syncObjId, object value)
        {
            var providerMock = new Mock<ISynchronizedObjectProvider>();
            providerMock.SetupGet(x => x.Id).Returns(syncObjId.Id);
            providerMock.Setup(x => x.FetchValue(ConferenceId, syncObjId)).ReturnsAsync(value);

            _providers.Add(providerMock.Object);
        }

        private void ParticipantHasSubscribed(string participantId, SynchronizedObjectId subscribed)
        {
            if (!_participantSubscriptions.TryGetValue(participantId, out var subscriptions))
                _participantSubscriptions[participantId] = subscriptions = new List<string>();

            subscriptions.Add(subscribed.ToString());

            _subscriptionsRepo.Setup(x => x.GetOfConference(ConferenceId)).ReturnsAsync(
                _participantSubscriptions.ToDictionary(x => x.Key, x => (IReadOnlyList<string>?) x.Value));
        }

        [Fact]
        public async Task Handle_ProviderDoesNotExist_ThrowException()
        {
            // arrange
            var syncObjId = new SynchronizedObjectId("testSyncId");
            var useCase = Create();

            // act
            var request = new UpdateSynchronizedObjectRequest(ConferenceId, syncObjId);
            await Assert.ThrowsAnyAsync<Exception>(async () => await useCase.Handle(request, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_SingleParticipantHasSubscribed_PublishSynchronizedObjectUpdatedNotification()
        {
            const string participantId = "34";
            var syncObjValue = "testValue";

            // arrange
            var syncObjId = new SynchronizedObjectId("testSyncId");
            SetupProvider(syncObjId, syncObjValue);
            ParticipantHasSubscribed(participantId, syncObjId);

            var capturedNotification = _mediator.CaptureNotification<SynchronizedObjectUpdatedNotification>();

            var useCase = Create();

            // act
            var request = new UpdateSynchronizedObjectRequest(ConferenceId, syncObjId);
            await useCase.Handle(request, CancellationToken.None);

            // assert
            capturedNotification.AssertReceived();

            var notification = capturedNotification.GetNotification();
            Assert.Equal(ConferenceId, notification.ConferenceId);
            Assert.Equal(participantId, Assert.Single(notification.ParticipantIds));
            Assert.Equal(syncObjId.ToString(), notification.SyncObjId);
            Assert.Equal(syncObjValue, notification.Value);
        }

        [Fact]
        public async Task Handle_NoSubscriptions_DoNothing()
        {
            var syncObjValue = "testValue";

            // arrange
            var syncObjId = new SynchronizedObjectId("testSyncId");
            SetupProvider(syncObjId, syncObjValue);

            _subscriptionsRepo.Setup(x => x.GetOfConference(ConferenceId))
                .ReturnsAsync(ImmutableDictionary<string, IReadOnlyList<string>?>.Empty);

            var useCase = Create();

            // act
            var request = new UpdateSynchronizedObjectRequest(ConferenceId, syncObjId);
            await useCase.Handle(request, CancellationToken.None);

            // assert
            _mediator.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task Handle_MultipleParticipantsSameSyncObject_PublishSingleSynchronizedObjectUpdatedNotification()
        {
            const string participantId = "34";
            const string participantId2 = "45";
            var syncObjValue = "testValue";

            // arrange
            var syncObjId = new SynchronizedObjectId("testSyncId");

            SetupProvider(syncObjId, syncObjValue);

            ParticipantHasSubscribed(participantId, syncObjId);
            ParticipantHasSubscribed(participantId2, syncObjId);

            var capturedNotification = _mediator.CaptureNotification<SynchronizedObjectUpdatedNotification>();

            var useCase = Create();

            // act
            var request = new UpdateSynchronizedObjectRequest(ConferenceId, syncObjId);
            await useCase.Handle(request, CancellationToken.None);

            // assert
            capturedNotification.AssertReceived();

            var notification = capturedNotification.GetNotification();
            AssertHelper.AssertScrambledEquals(new[] {participantId, participantId2}, notification.ParticipantIds);
        }
    }
}
