using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using Strive.Core.Extensions;
using Strive.Core.Services;
using Strive.Core.Services.Rooms;
using Strive.Core.Services.Rooms.Gateways;
using Strive.Core.Services.Rooms.Notifications;
using Strive.Core.Services.Rooms.Requests;
using Strive.Core.Services.Rooms.UseCases;
using Strive.Tests.Utils;
using Xunit;
using Xunit.Abstractions;

namespace Strive.Core.Tests.Services.Rooms.UseCases
{
    public class RemoveRoomsUseCaseTests
    {
        private const string ConferenceId = "123";

        private readonly Mock<IRoomRepository> _roomRepo = new();
        private readonly Mock<IMediator> _mediator = new();
        private readonly ILogger<RemoveRoomsUseCase> _logger;

        public RemoveRoomsUseCaseTests(ITestOutputHelper testOutputHelper)
        {
            _logger = testOutputHelper.CreateLogger<RemoveRoomsUseCase>();
        }

        private RemoveRoomsUseCase Create()
        {
            return new(_roomRepo.Object, _mediator.Object, _logger);
        }

        private void SetRemoveRoomIsSuccessful(string roomId)
        {
            _roomRepo.Setup(x => x.RemoveRoom(ConferenceId, roomId)).ReturnsAsync(true);
        }

        private void SetParticipantsInRoom(string roomId, IEnumerable<Participant> participants)
        {
            _roomRepo.Setup(x => x.GetParticipantsOfRoom(ConferenceId, roomId)).ReturnsAsync(participants.ToList());
        }

        [Fact]
        public async Task Handle_DeleteDefaultRoom_DontDeleteRoom()
        {
            const string roomId = RoomOptions.DEFAULT_ROOM_ID;

            // arrange
            var handler = Create();

            // act
            await handler.Handle(new RemoveRoomsRequest(ConferenceId, new[] {roomId}), CancellationToken.None);

            // assert
            _roomRepo.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task Handle_DeletionIsNotSuccessful_DontPublishNotification()
        {
            const string roomId = "test";

            // arrange
            var handler = Create();

            // act
            await handler.Handle(new RemoveRoomsRequest(ConferenceId, new[] {roomId}), CancellationToken.None);

            // assert
            _mediator.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task Handle_DeleteRoomWithNoParticipants_DeleteAndPublishNotification()
        {
            const string roomId = "test";

            // arrange
            SetRemoveRoomIsSuccessful(roomId);
            SetParticipantsInRoom(roomId, Enumerable.Empty<Participant>());

            var capturedNotification = _mediator.CaptureNotification<RoomsRemovedNotification>();

            var handler = Create();

            // act
            await handler.Handle(new RemoveRoomsRequest(ConferenceId, new[] {roomId}), CancellationToken.None);

            // assert
            _roomRepo.Verify(x => x.RemoveRoom(ConferenceId, roomId), Times.Once);

            capturedNotification.AssertReceived();
            var notification = capturedNotification.GetNotification();
            Assert.Equal(ConferenceId, notification.ConferenceId);
            Assert.Equal(roomId, Assert.Single(notification.RemovedRoomIds));
        }

        [Fact]
        public async Task Handle_DeleteRoomWithParticipants_MoveParticipantsToDefaultRoom()
        {
            const string roomId = "test";

            // arrange
            var testParticipant = new Participant(ConferenceId, "test");

            var capturedRequest = _mediator.CaptureRequest<SetParticipantRoomRequest, Unit>();

            SetRemoveRoomIsSuccessful(roomId);
            SetParticipantsInRoom(roomId, testParticipant.Yield());

            var handler = Create();

            // act
            await handler.Handle(new RemoveRoomsRequest(ConferenceId, new[] {roomId}), CancellationToken.None);

            // assert
            capturedRequest.AssertReceived();

            var request = capturedRequest.GetRequest();
            Assert.Equal(RoomOptions.DEFAULT_ROOM_ID, request.RoomId);
            Assert.Equal(testParticipant, request.Participant);
        }
    }
}
