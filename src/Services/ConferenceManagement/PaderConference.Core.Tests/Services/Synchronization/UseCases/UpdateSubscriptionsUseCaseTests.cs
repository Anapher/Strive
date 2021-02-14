using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using PaderConference.Core.Services.Synchronization;
using PaderConference.Core.Services.Synchronization.Gateways;
using PaderConference.Core.Services.Synchronization.Notifications;
using PaderConference.Core.Services.Synchronization.Requests;
using PaderConference.Core.Services.Synchronization.UseCases;
using PaderConference.Core.Tests._TestUtils;
using Xunit;
using Xunit.Abstractions;

namespace PaderConference.Core.Tests.Services.Synchronization.UseCases
{
    public class UpdateSubscriptionsUseCaseTests
    {
        private readonly ILogger<UpdateSubscriptionsUseCase> _logger;
        private readonly Mock<ISynchronizedObjectSubscriptionsRepository> _subscriptionRepo = new();
        private readonly Mock<IMediator> _mediator = new();
        private readonly List<ISynchronizedObjectProvider> _providers = new();

        private const string ConferenceId = "123";
        private const string ParticipantId = "45";

        public UpdateSubscriptionsUseCaseTests(ITestOutputHelper outputHelper)
        {
            _logger = outputHelper.CreateLogger<UpdateSubscriptionsUseCase>();
        }

        private UpdateSubscriptionsUseCase Create()
        {
            return new(_subscriptionRepo.Object, _providers, _mediator.Object, _logger);
        }

        [Fact]
        public async Task Handle_NoSubscriptionsAndNoProviders_DoNothing()
        {
            // arrange
            var useCase = Create();

            // act
            await useCase.Handle(new UpdateSubscriptionsRequest(ConferenceId, ParticipantId), CancellationToken.None);

            // assert
            _mediator.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task Handle_ProviderCannotSubscribe_DoNothing()
        {
            // arrange
            var provider = new Mock<ISynchronizedObjectProvider>();
            provider.Setup(x => x.CanSubscribe(ConferenceId, ParticipantId)).ReturnsAsync(false);

            var useCase = Create();

            // act
            await useCase.Handle(new UpdateSubscriptionsRequest(ConferenceId, ParticipantId), CancellationToken.None);

            // assert
            _mediator.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task Handle_HasProvider_PublishSynchronizedObjectUpdatedNotification()
        {
            const string syncObjId = "test";
            const string syncObjValue = "yikes";

            // arrange
            var capturedNotification = _mediator.CaptureNotification<SynchronizedObjectUpdatedNotification>();

            var provider = new Mock<ISynchronizedObjectProvider>();
            provider.Setup(x => x.CanSubscribe(ConferenceId, ParticipantId)).ReturnsAsync(true);
            provider.Setup(x => x.GetSynchronizedObjectId(ConferenceId, ParticipantId)).ReturnsAsync(syncObjId);
            provider.Setup(x => x.FetchValue(ConferenceId, ParticipantId)).ReturnsAsync(syncObjValue);
            _providers.Add(provider.Object);

            var useCase = Create();

            // act
            await useCase.Handle(new UpdateSubscriptionsRequest(ConferenceId, ParticipantId), CancellationToken.None);

            // assert
            capturedNotification.AssertReceived();

            var notification = capturedNotification.GetNotification();
            Assert.Equal(ConferenceId, notification.ConferenceId);
            Assert.Equal(ParticipantId, Assert.Single(notification.ParticipantIds));
            Assert.Equal(syncObjId, notification.SyncObjId);
            Assert.Equal(syncObjValue, notification.Value);
            Assert.Null(notification.PreviousValue);
        }

        [Fact]
        public async Task Handle_HasProvider_SetRepository()
        {
            const string syncObjId = "test";

            // arrange
            var provider = new Mock<ISynchronizedObjectProvider>();
            provider.Setup(x => x.CanSubscribe(ConferenceId, ParticipantId)).ReturnsAsync(true);
            provider.Setup(x => x.GetSynchronizedObjectId(ConferenceId, ParticipantId)).ReturnsAsync(syncObjId);
            _providers.Add(provider.Object);

            var useCase = Create();

            // act
            await useCase.Handle(new UpdateSubscriptionsRequest(ConferenceId, ParticipantId), CancellationToken.None);

            // assert
            _subscriptionRepo.Verify(
                x => x.GetSet(ConferenceId, ParticipantId,
                    It.Is<IReadOnlyList<string>>(x => x.Count == 1 && x[0] == syncObjId)), Times.Once);
        }

        [Fact]
        public async Task Handle_HasProviderAndAlreadySubscribed_DoNothing()
        {
            const string syncObjId = "test";

            // arrange
            var provider = new Mock<ISynchronizedObjectProvider>();
            provider.Setup(x => x.CanSubscribe(ConferenceId, ParticipantId)).ReturnsAsync(true);
            provider.Setup(x => x.GetSynchronizedObjectId(ConferenceId, ParticipantId)).ReturnsAsync(syncObjId);
            _providers.Add(provider.Object);

            _subscriptionRepo.Setup(x => x.GetSet(ConferenceId, ParticipantId, It.IsAny<IReadOnlyList<string>>()))
                .ReturnsAsync(new List<string> {syncObjId});

            var useCase = Create();

            // act
            await useCase.Handle(new UpdateSubscriptionsRequest(ConferenceId, ParticipantId), CancellationToken.None);

            // assert
            _mediator.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task Handle_HasProviderAndDifferentSubscribed_PublishParticipantSubscriptionsRemovedNotification()
        {
            const string syncObjId = "test";
            const string oldSyncObjId = "test2";

            // arrange
            var provider = new Mock<ISynchronizedObjectProvider>();
            provider.Setup(x => x.CanSubscribe(ConferenceId, ParticipantId)).ReturnsAsync(true);
            provider.Setup(x => x.GetSynchronizedObjectId(ConferenceId, ParticipantId)).ReturnsAsync(syncObjId);
            _providers.Add(provider.Object);

            _subscriptionRepo.Setup(x => x.GetSet(ConferenceId, ParticipantId, It.IsAny<IReadOnlyList<string>>()))
                .ReturnsAsync(new List<string> {oldSyncObjId});

            var capturedNotification = _mediator.CaptureNotification<ParticipantSubscriptionsRemovedNotification>();

            var useCase = Create();

            // act
            await useCase.Handle(new UpdateSubscriptionsRequest(ConferenceId, ParticipantId), CancellationToken.None);

            // assert
            capturedNotification.AssertReceived();

            var notification = capturedNotification.GetNotification();
            Assert.Equal(ConferenceId, notification.ConferenceId);
            Assert.Equal(ParticipantId, notification.ParticipantId);
            Assert.Equal(oldSyncObjId, Assert.Single(notification.RemovedSubscriptions));
        }
    }
}
