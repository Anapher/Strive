using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Moq;
using PaderConference.Core.Extensions;
using PaderConference.Core.Services.Synchronization;
using PaderConference.Core.Services.Synchronization.Gateways;
using PaderConference.Core.Services.Synchronization.Notifications;
using PaderConference.Core.Services.Synchronization.Requests;
using PaderConference.Core.Services.Synchronization.UseCases;
using PaderConference.Core.Tests._TestUtils;
using Xunit;

namespace PaderConference.Core.Tests.Services.Synchronization.UseCases
{
    public class UpdateSubscriptionsUseCaseTests
    {
        private readonly Mock<ISynchronizedObjectSubscriptionsRepository> _subscriptionRepo = new();
        private readonly Mock<IMediator> _mediator = new();
        private readonly List<ISynchronizedObjectProvider> _providers = new();

        private const string ConferenceId = "123";
        private const string ParticipantId = "45";

        private UpdateSubscriptionsUseCase Create()
        {
            return new(_subscriptionRepo.Object, _providers, _mediator.Object);
        }

        [Fact]
        public async Task Handle_NoProviders_DoNothing()
        {
            // arrange
            var useCase = Create();

            // act
            await useCase.Handle(new UpdateSubscriptionsRequest(ConferenceId, ParticipantId), CancellationToken.None);

            // assert
            _mediator.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task Handle_ProviderNoAvailableObjects_DoNothing()
        {
            // arrange
            var provider = new Mock<ISynchronizedObjectProvider>();
            provider.Setup(x => x.GetAvailableObjects(ConferenceId, ParticipantId))
                .ReturnsAsync(Enumerable.Empty<SynchronizedObjectId>());

            var useCase = Create();

            // act
            await useCase.Handle(new UpdateSubscriptionsRequest(ConferenceId, ParticipantId), CancellationToken.None);

            // assert
            _mediator.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task Handle_HasProvider_PublishSynchronizedObjectUpdatedNotification()
        {
            const string syncObjValue = "yikes";

            // arrange
            var syncObjId = SynchronizedObjectId.Parse("test");
            var capturedNotification = _mediator.CaptureNotification<SynchronizedObjectUpdatedNotification>();

            var provider = new Mock<ISynchronizedObjectProvider>();
            provider.Setup(x => x.GetAvailableObjects(ConferenceId, ParticipantId)).ReturnsAsync(syncObjId.Yield());
            provider.Setup(x => x.FetchValue(ConferenceId, syncObjId)).ReturnsAsync(syncObjValue);
            provider.SetupGet(x => x.Id).Returns(syncObjId.Id);
            _providers.Add(provider.Object);

            var useCase = Create();

            // act
            await useCase.Handle(new UpdateSubscriptionsRequest(ConferenceId, ParticipantId), CancellationToken.None);

            // assert
            capturedNotification.AssertReceived();

            var notification = capturedNotification.GetNotification();
            Assert.Equal(ConferenceId, notification.ConferenceId);
            Assert.Equal(ParticipantId, Assert.Single(notification.ParticipantIds));
            Assert.Equal(syncObjId.ToString(), notification.SyncObjId);
            Assert.Equal(syncObjValue, notification.Value);
            Assert.Null(notification.PreviousValue);
        }

        [Fact]
        public async Task Handle_HasProvider_SetRepository()
        {
            // arrange
            var syncObjId = SynchronizedObjectId.Parse("test");

            var provider = new Mock<ISynchronizedObjectProvider>();
            provider.Setup(x => x.GetAvailableObjects(ConferenceId, ParticipantId)).ReturnsAsync(syncObjId.Yield());
            provider.SetupGet(x => x.Id).Returns(syncObjId.Id);
            _providers.Add(provider.Object);

            var useCase = Create();

            // act
            await useCase.Handle(new UpdateSubscriptionsRequest(ConferenceId, ParticipantId), CancellationToken.None);

            // assert
            _subscriptionRepo.Verify(
                x => x.GetSet(ConferenceId, ParticipantId,
                    It.Is<IReadOnlyList<string>>(x => x.Count == 1 && x[0] == syncObjId.ToString())), Times.Once);
        }

        [Fact]
        public async Task Handle_HasProviderAndAlreadySubscribed_DoNothing()
        {
            // arrange
            var syncObjId = SynchronizedObjectId.Parse("test");

            var provider = new Mock<ISynchronizedObjectProvider>();
            provider.Setup(x => x.GetAvailableObjects(ConferenceId, ParticipantId)).ReturnsAsync(syncObjId.Yield());
            _providers.Add(provider.Object);

            _subscriptionRepo.Setup(x => x.GetSet(ConferenceId, ParticipantId, It.IsAny<IReadOnlyList<string>>()))
                .ReturnsAsync(new List<string> {syncObjId.ToString()});

            var useCase = Create();

            // act
            await useCase.Handle(new UpdateSubscriptionsRequest(ConferenceId, ParticipantId), CancellationToken.None);

            // assert
            _mediator.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task Handle_HasProviderAndDifferentSubscribed_PublishParticipantSubscriptionsRemovedNotification()
        {
            // arrange
            var syncObjId = SynchronizedObjectId.Parse("test");
            var oldSyncObjId = SynchronizedObjectId.Parse("test2");

            var provider = new Mock<ISynchronizedObjectProvider>();
            provider.Setup(x => x.GetAvailableObjects(ConferenceId, ParticipantId)).ReturnsAsync(syncObjId.Yield());
            provider.SetupGet(x => x.Id).Returns(syncObjId.Id);
            _providers.Add(provider.Object);

            _subscriptionRepo.Setup(x => x.GetSet(ConferenceId, ParticipantId, It.IsAny<IReadOnlyList<string>>()))
                .ReturnsAsync(new List<string> {oldSyncObjId.ToString()});

            var capturedNotification = _mediator.CaptureNotification<ParticipantSubscriptionsRemovedNotification>();

            var useCase = Create();

            // act
            await useCase.Handle(new UpdateSubscriptionsRequest(ConferenceId, ParticipantId), CancellationToken.None);

            // assert
            capturedNotification.AssertReceived();

            var notification = capturedNotification.GetNotification();
            Assert.Equal(ConferenceId, notification.ConferenceId);
            Assert.Equal(ParticipantId, notification.ParticipantId);
            Assert.Equal(oldSyncObjId.ToString(), Assert.Single(notification.RemovedSubscriptions));
        }
    }
}
