using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Options;
using Moq;
using Strive.Core.Services;
using Strive.Core.Services.ConferenceControl.Gateways;
using Strive.Core.Services.ConferenceControl.Notifications;
using Strive.Core.Services.Rooms;
using Strive.Core.Services.Rooms.Gateways;
using Strive.Core.Services.Rooms.NotificationHandlers;
using Strive.Core.Services.Rooms.Requests;
using Strive.Tests.Utils;
using Xunit;

namespace Strive.Core.Tests.Services.Rooms.NotificationHandlers
{
    public class ConferenceOpenedNotificationHandlerTests
    {
        private readonly Mock<IJoinedParticipantsRepository> _joinedParticipants = new();
        private readonly Mock<IRoomRepository> _roomRepo = new();
        private readonly Mock<IMediator> _mediator = new();
        private readonly RoomOptions _roomOptions = new();

        private const string ConferenceId = "123";

        private ConferenceOpenedNotificationHandler Create()
        {
            return new(_joinedParticipants.Object, _roomRepo.Object, _mediator.Object,
                new OptionsWrapper<RoomOptions>(_roomOptions));
        }

        private void SetParticipantsJoined(params Participant[] participants)
        {
            _joinedParticipants.Setup(x => x.GetParticipantsOfConference(ConferenceId)).ReturnsAsync(participants);
        }

        [Fact]
        public async Task Handle_CreateDefaultRoom()
        {
            var handler = Create();
            SetParticipantsJoined();

            // act
            await handler.Handle(new ConferenceOpenedNotification(ConferenceId), CancellationToken.None);

            // assert
            _roomRepo.Verify(
                x => x.CreateRoom(ConferenceId,
                    It.Is<Room>(room =>
                        room.RoomId == RoomOptions.DEFAULT_ROOM_ID &&
                        room.DisplayName == _roomOptions.DefaultRoomName)), Times.Once);
        }

        [Fact]
        public async Task Handle_MoveParticipantsToDefaultRoom()
        {
            var testParticipant = new Participant(ConferenceId, "asd");

            var capturedRequest = _mediator.CaptureRequest<SetParticipantRoomRequest, Unit>();

            var handler = Create();
            SetParticipantsJoined(testParticipant);

            // act
            await handler.Handle(new ConferenceOpenedNotification(ConferenceId), CancellationToken.None);

            // assert
            capturedRequest.AssertReceived();
            var request = capturedRequest.GetRequest();
            Assert.Equal(RoomOptions.DEFAULT_ROOM_ID, request.RoomId);
            Assert.Equal(testParticipant, request.Participant);
        }
    }
}
