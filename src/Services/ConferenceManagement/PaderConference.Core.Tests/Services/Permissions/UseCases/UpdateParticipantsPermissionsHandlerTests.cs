using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json.Linq;
using PaderConference.Core.Services.ConferenceControl.Gateways;
using PaderConference.Core.Services.Permissions;
using PaderConference.Core.Services.Permissions.Gateways;
using PaderConference.Core.Services.Permissions.Notifications;
using PaderConference.Core.Services.Permissions.Requests;
using PaderConference.Core.Services.Permissions.UseCases;
using PaderConference.Tests.Utils;
using Xunit;
using Xunit.Abstractions;

namespace PaderConference.Core.Tests.Services.Permissions.UseCases
{
    public class UpdateParticipantsPermissionsHandlerTests
    {
        private readonly ILogger<UpdateParticipantsPermissionsHandler> _logger;
        private readonly Mock<IAggregatedPermissionRepository> _aggregatedPermissions = new();
        private readonly Mock<IJoinedParticipantsRepository> _joinedParticipants = new();
        private readonly Mock<IPermissionLayersAggregator> _permissionLayersAggregator = new();
        private readonly Mock<IMediator> _mediator = new();

        private const string ConferenceId = "abc";
        private const string ParticipantId = "123";

        private readonly KeyValuePair<string, JValue> _somePermission =
            new("canDoSomething", (JValue) JToken.FromObject(true));

        public UpdateParticipantsPermissionsHandlerTests(ITestOutputHelper testOutputHelper)
        {
            _logger = testOutputHelper.CreateLogger<UpdateParticipantsPermissionsHandler>();
        }

        private UpdateParticipantsPermissionsHandler Create()
        {
            return new(_aggregatedPermissions.Object, _joinedParticipants.Object, _permissionLayersAggregator.Object,
                _mediator.Object, _logger);
        }

        private void SetParticipantJoined(string participantId)
        {
            _joinedParticipants.Setup(x => x.IsParticipantJoined(participantId, ConferenceId)).ReturnsAsync(true);
        }

        private void SetParticipantHasPermission(string participantId,
            params KeyValuePair<string, JValue>[] permissions)
        {
            _permissionLayersAggregator.Setup(x => x.FetchAggregatedPermissions(ConferenceId, participantId))
                .ReturnsAsync(new Dictionary<string, JValue>(permissions));
        }

        [Fact]
        public async Task Handle_ParticipantJoined_PublishNotification()
        {
            // arrange
            SetParticipantJoined(ParticipantId);

            var capturedNotification = _mediator.CaptureNotification<ParticipantPermissionsUpdatedNotification>();

            var handler = Create();
            var request = new UpdateParticipantsPermissionsRequest(ConferenceId, new[] {ParticipantId});

            SetParticipantHasPermission(ParticipantId, _somePermission);

            // act
            await handler.Handle(request, CancellationToken.None);

            // assert
            capturedNotification.AssertReceived();

            var notification = capturedNotification.GetNotification();
            Assert.Equal(ConferenceId, notification.ConferenceId);

            var participantUpdate = Assert.Single(notification.UpdatedPermissions);
            Assert.Equal(ParticipantId, participantUpdate.Key);
            Assert.Equal(_somePermission, Assert.Single(participantUpdate.Value));
        }

        [Fact]
        public async Task Handle_ParticipantJoined_UpdateRepository()
        {
            // arrange
            SetParticipantJoined(ParticipantId);

            var handler = Create();
            var request = new UpdateParticipantsPermissionsRequest(ConferenceId, new[] {ParticipantId});

            SetParticipantHasPermission(ParticipantId, _somePermission);

            // act
            await handler.Handle(request, CancellationToken.None);

            // assert
            _aggregatedPermissions.Verify(
                x => x.SetPermissions(ConferenceId, ParticipantId, It.IsAny<Dictionary<string, JValue>>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ParticipantNotJoined_DontPublish()
        {
            // arrange
            var handler = Create();
            var request = new UpdateParticipantsPermissionsRequest(ConferenceId, new[] {ParticipantId});

            SetParticipantHasPermission(ParticipantId, _somePermission);

            // act
            await handler.Handle(request, CancellationToken.None);

            // assert
            _mediator.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task Handle_ParticipantNotJoined_DeletePermissions()
        {
            // arrange
            var handler = Create();
            var request = new UpdateParticipantsPermissionsRequest(ConferenceId, new[] {ParticipantId});

            SetParticipantHasPermission(ParticipantId, _somePermission);

            // act
            await handler.Handle(request, CancellationToken.None);

            // assert
            _aggregatedPermissions.Verify(x => x.DeletePermissions(ConferenceId, ParticipantId), Times.Once);
        }

        [Fact]
        public async Task
            Handle_OneParticipantJoinedOneParticipantNotJoined_PublishNotificationOnlyWithJoinedParticipant()
        {
            const string participantNotJoined = "test1";

            // arrange
            SetParticipantJoined(ParticipantId);

            var capturedNotification = _mediator.CaptureNotification<ParticipantPermissionsUpdatedNotification>();

            var handler = Create();
            var request =
                new UpdateParticipantsPermissionsRequest(ConferenceId, new[] {ParticipantId, participantNotJoined});

            SetParticipantHasPermission(ParticipantId, _somePermission);
            SetParticipantHasPermission(participantNotJoined, _somePermission);

            // act
            await handler.Handle(request, CancellationToken.None);

            // assert
            capturedNotification.AssertReceived();
            var notification = capturedNotification.GetNotification();

            Assert.Equal(ParticipantId, Assert.Single(notification.UpdatedPermissions.Keys));
        }

        [Fact]
        public async Task Handle_TwoParticipants_PublishNotificationWithBothParticipants()
        {
            const string participant2 = "test1";

            // arrange
            SetParticipantJoined(ParticipantId);
            SetParticipantJoined(participant2);

            var capturedNotification = _mediator.CaptureNotification<ParticipantPermissionsUpdatedNotification>();

            var handler = Create();
            var request = new UpdateParticipantsPermissionsRequest(ConferenceId, new[] {ParticipantId, participant2});

            SetParticipantHasPermission(ParticipantId, _somePermission);
            SetParticipantHasPermission(participant2, _somePermission);

            // act
            await handler.Handle(request, CancellationToken.None);

            // assert
            capturedNotification.AssertReceived();

            var notification = capturedNotification.GetNotification();
            AssertHelper.AssertScrambledEquals(new[] {ParticipantId, participant2},
                notification.UpdatedPermissions.Keys);
        }
    }
}
