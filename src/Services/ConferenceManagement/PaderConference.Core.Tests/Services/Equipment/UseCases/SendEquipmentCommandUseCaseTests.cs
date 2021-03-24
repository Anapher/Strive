using System;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Moq;
using PaderConference.Core.Services;
using PaderConference.Core.Services.Equipment;
using PaderConference.Core.Services.Equipment.Gateways;
using PaderConference.Core.Services.Equipment.Notifications;
using PaderConference.Core.Services.Equipment.Requests;
using PaderConference.Core.Services.Equipment.UseCases;
using PaderConference.Core.Services.Media.Dtos;
using PaderConference.Tests.Utils;
using Xunit;

namespace PaderConference.Core.Tests.Services.Equipment.UseCases
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
