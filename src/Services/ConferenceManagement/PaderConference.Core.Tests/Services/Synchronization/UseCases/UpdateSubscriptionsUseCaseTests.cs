using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using PaderConference.Core.Extensions;
using PaderConference.Core.Services;
using PaderConference.Core.Services.ConferenceControl.Gateways;
using PaderConference.Core.Services.Synchronization;
using PaderConference.Core.Services.Synchronization.Gateways;
using PaderConference.Core.Services.Synchronization.Notifications;
using PaderConference.Core.Services.Synchronization.Requests;
using PaderConference.Core.Services.Synchronization.UseCases;
using PaderConference.Tests.Utils;
using Xunit;
using Xunit.Abstractions;

namespace PaderConference.Core.Tests.Services.Synchronization.UseCases
{
    public class UpdateSubscriptionsUseCaseTests
    {
        private const string ConferenceId = "123";
        private const string ParticipantId = "45";

        private static readonly Participant _participant = new(ConferenceId, ParticipantId);

        private readonly Mock<ISynchronizedObjectSubscriptionsRepository> _subscriptionRepo = new();
        private readonly Mock<ISynchronizedObjectRepository> _syncObjRepo = new();
        private readonly Mock<IMediator> _mediator = new();
        private readonly List<ISynchronizedObjectProvider> _providers = new();
        private readonly Mock<IJoinedParticipantsRepository> _joinedParticipantsRepo = new();
        private readonly ILogger<UpdateSubscriptionsUseCase> _logger;

        public UpdateSubscriptionsUseCaseTests(ITestOutputHelper testOutputHelper)
        {
            _logger = testOutputHelper.CreateLogger<UpdateSubscriptionsUseCase>();
        }

        private UpdateSubscriptionsUseCase Create()
        {
            return new(_subscriptionRepo.Object, _syncObjRepo.Object, _joinedParticipantsRepo.Object, _providers,
                _mediator.Object, _logger);
        }

        private void SetIsParticipantJoined()
        {
            _joinedParticipantsRepo.Setup(x => x.IsParticipantJoined(_participant)).ReturnsAsync(true);
        }

        [Fact]
        public async Task Handle_NoProviders_DoNothing()
        {
            // arrange
            var useCase = Create();
            SetIsParticipantJoined();

            // act
            await useCase.Handle(new UpdateSubscriptionsRequest(_participant), CancellationToken.None);

            // assert
            _mediator.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task Handle_ProviderNoAvailableObjects_DoNothing()
        {
            // arrange
            var provider = new Mock<ISynchronizedObjectProvider>();
            provider.Setup(x => x.GetAvailableObjects(_participant))
                .ReturnsAsync(Enumerable.Empty<SynchronizedObjectId>());

            SetIsParticipantJoined();

            var useCase = Create();

            // act
            await useCase.Handle(new UpdateSubscriptionsRequest(_participant), CancellationToken.None);

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
            provider.Setup(x => x.GetAvailableObjects(_participant)).ReturnsAsync(syncObjId.Yield());
            provider.Setup(x => x.FetchValue(ConferenceId, syncObjId)).ReturnsAsync(syncObjValue);
            provider.SetupGet(x => x.Id).Returns(syncObjId.Id);
            _providers.Add(provider.Object);

            SetIsParticipantJoined();

            var useCase = Create();

            // act
            await useCase.Handle(new UpdateSubscriptionsRequest(_participant), CancellationToken.None);

            // assert
            capturedNotification.AssertReceived();

            var notification = capturedNotification.GetNotification();
            Assert.Equal(_participant, Assert.Single(notification.Participants));
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
            provider.Setup(x => x.GetAvailableObjects(_participant)).ReturnsAsync(syncObjId.Yield());
            provider.SetupGet(x => x.Id).Returns(syncObjId.Id);
            _providers.Add(provider.Object);

            SetIsParticipantJoined();

            var useCase = Create();

            // act
            await useCase.Handle(new UpdateSubscriptionsRequest(_participant), CancellationToken.None);

            // assert
            _subscriptionRepo.Verify(
                x => x.GetSet(_participant,
                    It.Is<IReadOnlyList<string>>(x => x.Count == 1 && x[0] == syncObjId.ToString())), Times.Once);
        }

        [Fact]
        public async Task Handle_HasProviderAndAlreadySubscribed_DoNothing()
        {
            // arrange
            var syncObjId = SynchronizedObjectId.Parse("test");

            var provider = new Mock<ISynchronizedObjectProvider>();
            provider.Setup(x => x.GetAvailableObjects(_participant)).ReturnsAsync(syncObjId.Yield());
            _providers.Add(provider.Object);

            _subscriptionRepo.Setup(x => x.GetSet(_participant, It.IsAny<IReadOnlyList<string>>()))
                .ReturnsAsync(new List<string> {syncObjId.ToString()});

            SetIsParticipantJoined();

            var useCase = Create();

            // act
            await useCase.Handle(new UpdateSubscriptionsRequest(_participant), CancellationToken.None);

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
            provider.Setup(x => x.GetAvailableObjects(_participant)).ReturnsAsync(syncObjId.Yield());
            provider.SetupGet(x => x.Id).Returns(syncObjId.Id);
            _providers.Add(provider.Object);

            _subscriptionRepo.Setup(x => x.GetSet(_participant, It.IsAny<IReadOnlyList<string>>()))
                .ReturnsAsync(new List<string> {oldSyncObjId.ToString()});

            var capturedNotification = _mediator.CaptureNotification<ParticipantSubscriptionsRemovedNotification>();

            var useCase = Create();

            SetIsParticipantJoined();

            // act
            await useCase.Handle(new UpdateSubscriptionsRequest(_participant), CancellationToken.None);

            // assert
            capturedNotification.AssertReceived();

            var notification = capturedNotification.GetNotification();
            Assert.Equal(_participant, notification.Participant);
            Assert.Equal(oldSyncObjId.ToString(), Assert.Single(notification.RemovedSubscriptions));
        }

        [Fact]
        public async Task Handle_ParticipantNotJoined_DontCreateSubscriptionsNorPublishNotifications()
        {
            const string syncObjValue = "yikes";

            // arrange
            var syncObjId = SynchronizedObjectId.Parse("test");

            var provider = new Mock<ISynchronizedObjectProvider>();
            provider.Setup(x => x.GetAvailableObjects(_participant)).ReturnsAsync(syncObjId.Yield());
            provider.Setup(x => x.FetchValue(ConferenceId, syncObjId)).ReturnsAsync(syncObjValue);
            provider.SetupGet(x => x.Id).Returns(syncObjId.Id);
            _providers.Add(provider.Object);

            var useCase = Create();

            // act
            await useCase.Handle(new UpdateSubscriptionsRequest(_participant), CancellationToken.None);

            // assert
            _mediator.VerifyNoOtherCalls();
            _subscriptionRepo.Verify(x => x.Remove(_participant), Times.Once);
        }
    }
}
