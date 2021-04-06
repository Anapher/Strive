using System;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Moq;
using Strive.Core.Services;
using Strive.Core.Services.Equipment;
using Strive.Core.Services.Equipment.Gateways;
using Strive.Core.Services.Equipment.Notifications;
using Strive.Core.Services.Equipment.Requests;
using Strive.Core.Services.Equipment.UseCases;
using Strive.Core.Services.Media.Dtos;
using Strive.Tests.Utils;
using Xunit;

namespace Strive.Core.Tests.Services.Equipment.UseCases
{
    public class SendEquipmentCommandUseCaseTests
    {
        private readonly Mock<IEquipmentConnectionRepository> _repo = new();
        private readonly Mock<IMediator> _mediator = new();

        private readonly Participant _testParticipant = new("123", "435");

        private SendEquipmentCommandUseCase Create()
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
                new SendEquipmentCommandRequest(_testParticipant, "test", ProducerSource.Mic, null,
                    EquipmentCommandType.Pause), CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ConnectionExists_PublishNotification()
        {
            const string connectionId = "test";

            // arrange
            var useCase = Create();
            var capturedNotification = _mediator.CaptureNotification<SendEquipmentCommandNotification>();

            _repo.Setup(x => x.GetConnection(_testParticipant, connectionId)).ReturnsAsync(
                new EquipmentConnection(connectionId, "test name", ImmutableDictionary<string, EquipmentDevice>.Empty,
                    ImmutableDictionary<string, UseMediaStateInfo>.Empty));

            // act
            await useCase.Handle(
                new SendEquipmentCommandRequest(_testParticipant, connectionId, ProducerSource.Mic, null,
                    EquipmentCommandType.Pause), CancellationToken.None);

            // assert
            capturedNotification.AssertReceived();
        }
    }
}
