using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Moq;
using Strive.Core.Services;
using Strive.Core.Services.ConferenceControl.Notifications;
using Strive.Core.Services.ConferenceControl.Requests;
using Strive.Core.Services.ConferenceControl.UseCases;
using Xunit;

namespace Strive.Core.Tests.Services.ConferenceControl.UseCases
{
    public class KickParticipantHandlerTests
    {
        private readonly Mock<IMediator> _mediator;

        public KickParticipantHandlerTests()
        {
            _mediator = new Mock<IMediator>();
        }

        private KickParticipantHandler CreateHandler()
        {
            return new(_mediator.Object);
        }

        [Fact]
        public async Task Handle_KickParticipant_ParticipantKickedNotificationIssued()
        {
            const string participantId = "123";
            const string conferenceId = "45";

            var participant = new Participant(conferenceId, participantId);

            // arrange
            var handler = CreateHandler();
            var request = new KickParticipantRequest(participant);

            // act
            await handler.Handle(request, CancellationToken.None);

            // assert
            _mediator.Verify(x => x.Publish(It.IsAny<ParticipantKickedNotification>(), It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}
