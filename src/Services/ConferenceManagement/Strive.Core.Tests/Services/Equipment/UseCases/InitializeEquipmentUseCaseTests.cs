using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Moq;
using Strive.Core.Services;
using Strive.Core.Services.ConferenceControl.Requests;
using Strive.Core.Services.Equipment;
using Strive.Core.Services.Equipment.Gateways;
using Strive.Core.Services.Equipment.Requests;
using Strive.Core.Services.Equipment.UseCases;
using Strive.Core.Services.Synchronization.Requests;
using Xunit;

namespace Strive.Core.Tests.Services.Equipment.UseCases
{
    public class InitializeEquipmentUseCaseTests
    {
        private readonly Mock<IMediator> _mediator = new();
        private readonly Mock<IEquipmentConnectionRepository> _repo = new();

        private readonly Participant _testParticipant = new("123", "wtf");
        private const string ConnectionId = "connId";
        private const string DeviceName = "Smartphone";

        private InitializeEquipmentUseCase Create()
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
        public async Task Handle_ParticipantJoined_AddConnectionInRepo()
        {
            // arrange
            var useCase = Create();

            var testDevice = new EquipmentDevice("123", "Smartphone Microphone", DeviceType.Mic);
            var request =
                new InitializeEquipmentRequest(_testParticipant, ConnectionId, DeviceName, new[] {testDevice});

            SetupIsParticipantJoined(_testParticipant, true);

            EquipmentConnection? addedConnection = null;
            _repo.Setup(x => x.SetConnection(_testParticipant, It.IsAny<EquipmentConnection>()))
                .Callback((Participant _, EquipmentConnection conn) => addedConnection = conn);

            // act
            await useCase.Handle(request, CancellationToken.None);

            // assert
            _repo.Verify(x => x.SetConnection(_testParticipant, It.IsAny<EquipmentConnection>()), Times.Once);

            Assert.NotNull(addedConnection);
            Assert.Equal(DeviceName, addedConnection!.Name);
            Assert.Equal(ConnectionId, addedConnection!.ConnectionId);
            Assert.Empty(addedConnection!.Status);
            var device = Assert.Single(addedConnection!.Devices);
            Assert.Equal(testDevice.DeviceId, device.Key);
            Assert.Equal(testDevice, device.Value);
        }

        [Fact]
        public async Task Handle_ParticipantNotJoined_ThrowExceptionAndRemoveAddedConnection()
        {
            // arrange
            var useCase = Create();

            var request = new InitializeEquipmentRequest(_testParticipant, ConnectionId, DeviceName,
                Array.Empty<EquipmentDevice>());

            SetupIsParticipantJoined(_testParticipant, false);

            // act
            await Assert.ThrowsAnyAsync<Exception>(async () => await useCase.Handle(request, CancellationToken.None));
            _repo.Verify(x => x.RemoveConnection(_testParticipant, ConnectionId), Times.Once);
        }

        [Fact]
        public async Task Handle_ParticipantJoined_UpdateSyncObject()
        {
            // arrange
            var useCase = Create();

            var request = new InitializeEquipmentRequest(_testParticipant, ConnectionId, DeviceName,
                Array.Empty<EquipmentDevice>());

            SetupIsParticipantJoined(_testParticipant, true);

            // act
            await useCase.Handle(request, CancellationToken.None);
            _mediator.Verify(
                x => x.Send(
                    It.Is<UpdateSynchronizedObjectRequest>(x =>
                        x.SynchronizedObjectId.ToString() == $"equipment?participantId={_testParticipant.Id}"),
                    It.IsAny<CancellationToken>()), Times.Once());
        }
    }
}
