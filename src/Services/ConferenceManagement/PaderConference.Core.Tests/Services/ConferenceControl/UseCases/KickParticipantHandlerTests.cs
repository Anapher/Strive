using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Moq;
using PaderConference.Core.Services.ConferenceControl.Notifications;
using PaderConference.Core.Services.ConferenceControl.Requests;
using PaderConference.Core.Services.ConferenceControl.UseCases;
using Xunit;

namespace PaderConference.Core.Tests.Services.ConferenceControl.UseCases
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

            // arrange
            var handler = CreateHandler();
            var request = new KickParticipantRequest(participantId, conferenceId);

            // act
            await handler.Handle(request, CancellationToken.None);

            // assert
            _mediator.Verify(x => x.Publish(It.IsAny<ParticipantKickedNotification>(), It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}
