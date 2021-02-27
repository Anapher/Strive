using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using PaderConference.Core.Services;
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
    public class UpdateSynchronizedObjectUseCaseTests
    {
        private readonly Mock<ISynchronizedObjectRepository> _syncObjRepo = new();
        private readonly Mock<IMediator> _mediator = new();
        private readonly List<ISynchronizedObjectProvider> _providers = new();
        private readonly Dictionary<string, List<Participant>> _participantSubscriptions = new();
        private readonly ILogger<UpdateSynchronizedObjectUseCase> _logger;

        private const string ConferenceId = "123";

        public UpdateSynchronizedObjectUseCaseTests(ITestOutputHelper testOutputHelper)
        {
            _logger = testOutputHelper.CreateLogger<UpdateSynchronizedObjectUseCase>();
        }

        public UpdateSynchronizedObjectUseCase Create()
        {
            return new(_providers, _syncObjRepo.Object, _mediator.Object, _logger);
        }

        private void SetupProvider(SynchronizedObjectId syncObjId, object value)
        {
            var providerMock = new Mock<ISynchronizedObjectProvider>();
            providerMock.SetupGet(x => x.Id).Returns(syncObjId.Id);
            providerMock.Setup(x => x.FetchValue(ConferenceId, syncObjId)).ReturnsAsync(value);

            _providers.Add(providerMock.Object);
        }

        private void ParticipantHasSubscribed(Participant participant, SynchronizedObjectId subscribed)
        {
            if (!_participantSubscriptions.TryGetValue(subscribed.ToString(), out var participants))
                _participantSubscriptions[subscribed.ToString()] = participants = new List<Participant>();

            participants.Add(participant);

            _mediator.Setup(x => x.Send(It.IsAny<FetchSubscribedParticipantsRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(participants);
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
            var participant = new Participant(ConferenceId, "123");
            var syncObjValue = "testValue";

            // arrange
            var syncObjId = new SynchronizedObjectId("testSyncId");
            SetupProvider(syncObjId, syncObjValue);
            ParticipantHasSubscribed(participant, syncObjId);

            var capturedNotification = _mediator.CaptureNotification<SynchronizedObjectUpdatedNotification>();

            var useCase = Create();

            // act
            var request = new UpdateSynchronizedObjectRequest(ConferenceId, syncObjId);
            await useCase.Handle(request, CancellationToken.None);

            // assert
            capturedNotification.AssertReceived();

            var notification = capturedNotification.GetNotification();
            Assert.Equal(participant, Assert.Single(notification.Participants));
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

            _mediator.Setup(x => x.Send(It.IsAny<FetchSubscribedParticipantsRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Array.Empty<Participant>());

            var useCase = Create();

            // act
            var request = new UpdateSynchronizedObjectRequest(ConferenceId, syncObjId);
            await useCase.Handle(request, CancellationToken.None);

            // assert
            _mediator.Verify(x => x.Send(It.IsAny<FetchSubscribedParticipantsRequest>(), It.IsAny<CancellationToken>()),
                Times.Once);
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

            ParticipantHasSubscribed(new Participant(ConferenceId, participantId), syncObjId);
            ParticipantHasSubscribed(new Participant(ConferenceId, participantId2), syncObjId);

            var capturedNotification = _mediator.CaptureNotification<SynchronizedObjectUpdatedNotification>();

            var useCase = Create();

            // act
            var request = new UpdateSynchronizedObjectRequest(ConferenceId, syncObjId);
            await useCase.Handle(request, CancellationToken.None);

            // assert
            capturedNotification.AssertReceived();

            var notification = capturedNotification.GetNotification();
            AssertHelper.AssertScrambledEquals(new[] {participantId, participantId2},
                notification.Participants.Select(x => x.Id));
        }
    }
}
