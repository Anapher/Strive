using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Moq;
using Strive.Core.Services;
using Strive.Core.Services.Equipment;
using Strive.Core.Services.Equipment.Gateways;
using Strive.Core.Services.Equipment.Requests;
using Strive.Core.Services.Equipment.UseCases;
using Strive.Core.Services.Media.Dtos;
using Strive.Core.Services.Synchronization.Requests;
using Strive.Tests.Utils;
using Xunit;

namespace Strive.Core.Tests.Services.Equipment.UseCases
{
    public class UpdateStatusUseCaseTests
    {
        private readonly Mock<IEquipmentConnectionRepository> _repo = new();
        private readonly Mock<IMediator> _mediator = new();

        private readonly Participant _testParticipant = new("123", "435");
        private const string ConnectionId = "test";

        private UpdateStatusUseCase Create()
        {
            return new(_repo.Object, _mediator.Object);
        }

        [Fact]
        public async Task Handle_ConnectionDoesNotExist_ThrowException()
        {
            // arrange
            var useCase = Create();

            // act
            await Assert.ThrowsAnyAsync<Exception>(async () => await useCase.Handle(
                new UpdateStatusRequest(_testParticipant, ConnectionId,
                    ImmutableDictionary<ProducerSource, UseMediaStateInfo>.Empty), CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ConnectionExists_UpdateInDatabase()
        {
            // arrange
            var useCase = Create();
            var existingConnection = new EquipmentConnection(ConnectionId, "Smartphone", Array.Empty<EquipmentDevice>(),
                ImmutableDictionary<ProducerSource, UseMediaStateInfo>.Empty);

            _repo.Setup(x => x.GetConnection(_testParticipant, ConnectionId)).ReturnsAsync(existingConnection);

            EquipmentConnection? addedConnection = null;
            _repo.Setup(x => x.SetConnection(_testParticipant, It.IsAny<EquipmentConnection>()))
                .Callback((Participant _, EquipmentConnection conn) => addedConnection = conn);

            var newMediaState = new Dictionary<ProducerSource, UseMediaStateInfo>
                {{ProducerSource.Mic, new UseMediaStateInfo(true, false, false, null)}};

            // act
            await useCase.Handle(new UpdateStatusRequest(_testParticipant, ConnectionId, newMediaState),
                CancellationToken.None);

            // assert
            Assert.NotNull(addedConnection);

            Assert.Equal(existingConnection.ConnectionId, addedConnection!.ConnectionId);
            Assert.Equal(existingConnection.Name, addedConnection!.Name);
            Assert.Equal(existingConnection.Devices, addedConnection!.Devices);
            Assert.Equal(newMediaState, addedConnection!.Status);
        }

        [Fact]
        public async Task Handle_ConnectionExists_UpdateSyncObj()
        {
            // arrange
            var useCase = Create();
            var existingConnection = new EquipmentConnection(ConnectionId, "Smartphone", Array.Empty<EquipmentDevice>(),
                ImmutableDictionary<ProducerSource, UseMediaStateInfo>.Empty);

            _repo.Setup(x => x.GetConnection(_testParticipant, ConnectionId)).ReturnsAsync(existingConnection);

            var capturedRequest = _mediator.CaptureRequest<UpdateSynchronizedObjectRequest, Unit>();

            // act
            await useCase.Handle(
                new UpdateStatusRequest(_testParticipant, ConnectionId,
                    ImmutableDictionary<ProducerSource, UseMediaStateInfo>.Empty), CancellationToken.None);

            // assert
            capturedRequest.AssertReceived();

            Assert.Equal($"equipment?participantId={_testParticipant.Id}",
                capturedRequest.GetRequest().SynchronizedObjectId.ToString());
        }
    }
}
