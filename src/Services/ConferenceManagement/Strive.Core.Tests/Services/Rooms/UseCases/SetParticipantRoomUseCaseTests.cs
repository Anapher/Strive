using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using Strive.Core.Interfaces.Gateways.Repositories;
using Strive.Core.Services;
using Strive.Core.Services.Rooms.Gateways;
using Strive.Core.Services.Rooms.Notifications;
using Strive.Core.Services.Rooms.Requests;
using Strive.Core.Services.Rooms.UseCases;
using Strive.Tests.Utils;
using Xunit;
using Xunit.Abstractions;

namespace Strive.Core.Tests.Services.Rooms.UseCases
{
    public class SetParticipantRoomUseCaseTests
    {
        private readonly Mock<IMediator> _mediator = new();
        private readonly Mock<IRoomRepository> _roomRepo = new();
        private readonly ILogger<SetParticipantRoomUseCase> _logger;

        private readonly Participant _testParticipant = new("13", "456");
        private const string NewRoomId = "1";

        public SetParticipantRoomUseCaseTests(ITestOutputHelper testOutputHelper)
        {
            _logger = testOutputHelper.CreateLogger<SetParticipantRoomUseCase>();
        }

        private SetParticipantRoomUseCase Create()
        {
            return new(_mediator.Object, _roomRepo.Object, _logger);
        }

        [Fact]
        public async Task Handle_SetParticipantRoomFails_NoNotification()
        {
            // arrange
            var handler = Create();
            _roomRepo.Setup(x => x.SetParticipantRoom(_testParticipant, NewRoomId))
                .Throws(new ConcurrencyException("yikes"));

            // act
            await Assert.ThrowsAsync<ConcurrencyException>(async () =>
                await handler.Handle(new SetParticipantRoomRequest(_testParticipant, NewRoomId),
                    CancellationToken.None));

            // assert
            _mediator.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task Handle_SetParticipant_PublishNotification()
        {
            // arrange
            var handler = Create();
            var capturedNotification = _mediator.CaptureNotification<ParticipantsRoomChangedNotification>();
            _roomRepo.Setup(x => x.SetParticipantRoom(_testParticipant, NewRoomId)).ReturnsAsync("oldRoom");

            // act
            await handler.Handle(new SetParticipantRoomRequest(_testParticipant, NewRoomId), CancellationToken.None);

            // assert
            capturedNotification.AssertReceived();

            var notification = capturedNotification.GetNotification();
            Assert.Equal(_testParticipant.ConferenceId, notification.ConferenceId);
            Assert.Single(notification.Participants,
                new KeyValuePair<Participant, ParticipantRoomChangeInfo>(_testParticipant,
                    ParticipantRoomChangeInfo.Switched("oldRoom", NewRoomId)));
        }
    }
}
