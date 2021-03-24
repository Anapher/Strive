using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Moq;
using PaderConference.Core.Services;
using PaderConference.Core.Services.ConferenceControl.Requests;
using PaderConference.Core.Services.Equipment.Gateways;
using PaderConference.Core.Services.Equipment.Requests;
using PaderConference.Core.Services.Equipment.UseCases;
using Xunit;

namespace PaderConference.Core.Tests.Services.Equipment.UseCases
{
    public class AuthenticateEquipmentUseCaseTests
    {
        private readonly Mock<IEquipmentTokenRepository> _repo = new();
        private readonly Mock<IMediator> _mediator = new();

        private readonly Participant _testParticipant = new("123", "45");

        private AuthenticateEquipmentUseCase Create()
        {
            return new(_repo.Object, _mediator.Object);
        }

        private void SetupIsParticipantJoined(Participant participant, bool joined)
        {
            _mediator.Setup(x =>
                x.Send(It.Is<CheckIsParticipantJoinedRequest>(request => request.Participant.Equals(participant)),
                    It.IsAny<CancellationToken>())).ReturnsAsync(joined);
        }

        [Fact]
        public async Task Handle_TokensDoNotMatch_ThrowException()
        {
            // arrange
            var useCase = Create();

            SetupIsParticipantJoined(_testParticipant, true);
            _repo.Setup(x => x.Get(_testParticipant)).ReturnsAsync("testToken");

            // act
            await Assert.ThrowsAnyAsync<Exception>(async () =>
            {
                await useCase.Handle(new AuthenticateEquipmentRequest(_testParticipant, "differentToken"),
                    CancellationToken.None);
            });
        }

        [Fact]
        public async Task Handle_ParticipantNotJoined_ThrowException()
        {
            const string token = "123";

            // arrange
            var useCase = Create();

            SetupIsParticipantJoined(_testParticipant, false);
            _repo.Setup(x => x.Get(_testParticipant)).ReturnsAsync(token);

            // act
            await Assert.ThrowsAnyAsync<Exception>(async () =>
            {
                await useCase.Handle(new AuthenticateEquipmentRequest(_testParticipant, token),
                    CancellationToken.None);
            });
        }

        [Fact]
        public async Task Handle_ValidTokenAndParticipantJoined_NoException()
        {
            const string token = "123";

            // arrange
            var useCase = Create();

            SetupIsParticipantJoined(_testParticipant, true);
            _repo.Setup(x => x.Get(_testParticipant)).ReturnsAsync(token);

            // act
            await useCase.Handle(new AuthenticateEquipmentRequest(_testParticipant, token), CancellationToken.None);
        }
    }
}
