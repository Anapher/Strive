using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using PaderConference.Core.Services;
using PaderConference.Core.Services.ConferenceControl.ClientControl;
using PaderConference.Core.Services.ConferenceControl.Gateways;
using PaderConference.Core.Services.ConferenceControl.Notifications;
using PaderConference.Core.Services.ConferenceControl.Requests;
using PaderConference.Core.Services.ConferenceControl.UseCases;
using PaderConference.Tests.Utils;
using Xunit;
using Xunit.Abstractions;

namespace PaderConference.Core.Tests.Services.ConferenceControl.UseCases
{
    public class JoinConferenceRequestHandlerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<IJoinedParticipantsRepository> _joinedParticipantsRepositoryMock;
        private readonly ILogger<JoinConferenceRequestHandler> _logger;

        private const string ConferenceId = "123";
        private const string ParticipantId = "34";
        private const string ConnectionId = "324";

        private static readonly Participant Participant = new(ConferenceId, ParticipantId);

        public JoinConferenceRequestHandlerTests(ITestOutputHelper outputHelper)
        {
            _mediatorMock = new Mock<IMediator>();
            _joinedParticipantsRepositoryMock = new Mock<IJoinedParticipantsRepository>();
            _logger = outputHelper.CreateLogger<JoinConferenceRequestHandler>();
        }

        private JoinConferenceRequestHandler Create()
        {
            return new(_mediatorMock.Object, _joinedParticipantsRepositoryMock.Object, _logger);
        }

        private JoinConferenceRequest CreateDefaultRequest()
        {
            return new(new Participant(ConferenceId, ParticipantId), ConnectionId);
        }

        [Fact]
        public async Task Handle_ParticipantNotJoined_IssueParticipantJoinedNotification()
        {
            // arrange
            var request = CreateDefaultRequest();
            var handler = Create();

            // act
            await handler.Handle(request, CancellationToken.None);

            // assert
            _mediatorMock.Verify(
                x => x.Send(It.IsAny<EnableParticipantMessagingRequest>(), It.IsAny<CancellationToken>()), Times.Once);
            _mediatorMock.Verify(
                x => x.Publish(It.IsAny<ParticipantJoinedNotification>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ParticipantNotJoined_EnableMessagingBeforeJoinNotification()
        {
            // arrange
            var request = CreateDefaultRequest();
            var handler = Create();

            var counter = 0;
            var timeEnableParticipantMessagingRequest = 0;
            var timeParticipantJoinedNotification = 0;

            _mediatorMock
                .Setup(x => x.Send(It.IsAny<EnableParticipantMessagingRequest>(), It.IsAny<CancellationToken>()))
                .Callback(() => timeEnableParticipantMessagingRequest = counter++);
            _mediatorMock
                .Setup(x => x.Publish(It.IsAny<ParticipantJoinedNotification>(), It.IsAny<CancellationToken>()))
                .Callback(() => timeParticipantJoinedNotification = counter++);

            // act
            await handler.Handle(request, CancellationToken.None);

            // assert
            Assert.True(timeEnableParticipantMessagingRequest < timeParticipantJoinedNotification);
        }

        [Fact]
        public async Task Handle_ParticipantAlreadyJoined_KickParticipant()
        {
            const string connectedConnectionId = "differentConnectionId";
            var participantToKick = new Participant("differentConference", ParticipantId);

            // arrange
            var capturedNotification = _mediatorMock.CaptureNotification<ParticipantKickedNotification>();

            _joinedParticipantsRepositoryMock.Setup(x => x.AddParticipant(Participant, ConnectionId))
                .ReturnsAsync(new PreviousParticipantState(participantToKick.ConferenceId, connectedConnectionId));

            var request = CreateDefaultRequest();
            var handler = Create();

            // act
            await handler.Handle(request, CancellationToken.None);

            // assert
            capturedNotification.AssertReceived();

            var notification = capturedNotification.GetNotification();
            Assert.Equal(participantToKick, notification.Participant);
            Assert.Equal(connectedConnectionId, notification.ConnectionId);
        }
    }
}
