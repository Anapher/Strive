using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using PaderConference.Core.Interfaces.Gateways.Repositories;
using PaderConference.Core.Services.ConferenceControl.Gateways;
using PaderConference.Core.Services.Rooms;
using PaderConference.Core.Services.Rooms.Gateways;
using PaderConference.Core.Services.Rooms.Notifications;
using PaderConference.Core.Services.Rooms.Requests;
using PaderConference.Core.Services.Rooms.UseCases;
using PaderConference.Tests.Utils;
using Xunit;
using Xunit.Abstractions;

namespace PaderConference.Core.Tests.Services.Rooms.UseCases
{
    public class CreateRoomsUseCaseTests
    {
        private const string ConferenceId = "123";

        private readonly ILogger<CreateRoomsUseCase> _logger;
        private readonly Mock<IRoomRepository> _roomRepo = new();
        private readonly Mock<IOpenConferenceRepository> _openConferenceRepo = new();
        private readonly Mock<IMediator> _mediator = new();

        public CreateRoomsUseCaseTests(ITestOutputHelper testOutputHelper)
        {
            _logger = testOutputHelper.CreateLogger<CreateRoomsUseCase>();
        }

        private CreateRoomsUseCase Create()
        {
            return new(_roomRepo.Object, _openConferenceRepo.Object, _mediator.Object, _logger);
        }

        private void SetConferenceIsOpen()
        {
            _openConferenceRepo.Setup(x => x.IsOpen(ConferenceId)).ReturnsAsync(true);
        }

        [Fact]
        public async Task Handle_ConferenceNotOpen_ConcurrencyExceptionAndNoRoomsCreatedAndNoNotification()
        {
            // arrange
            var handler = Create();

            Room? createdRoom = null;
            _roomRepo.Setup(x => x.CreateRoom(ConferenceId, It.IsAny<Room>()))
                .Callback((string _, Room room) => createdRoom = room);

            // act
            var createRoomInfo = new RoomCreationInfo("test");
            var request = new CreateRoomsRequest(ConferenceId, new[] {createRoomInfo});

            await Assert.ThrowsAsync<ConcurrencyException>(async () =>
                await handler.Handle(request, CancellationToken.None));

            // assert
            _mediator.VerifyNoOtherCalls();

            if (createdRoom != null)
                _roomRepo.Verify(x => x.RemoveRoom(ConferenceId, It.Is<string>(x => x == createdRoom.RoomId)),
                    Times.Once);
        }

        [Fact]
        public async Task Handle_ConferenceIsOpen_PublishNotification()
        {
            // arrange
            SetConferenceIsOpen();

            var handler = Create();
            var capturedNotification = _mediator.CaptureNotification<RoomsCreatedNotification>();

            // act
            var createRoomInfo = new RoomCreationInfo("test");
            var request = new CreateRoomsRequest(ConferenceId, new[] {createRoomInfo});

            await handler.Handle(request, CancellationToken.None);

            // assert
            capturedNotification.AssertReceived();

            var notification = capturedNotification.GetNotification();
            Assert.Equal(ConferenceId, notification.ConferenceId);
            Assert.Single(notification.CreatedRoomIds);
        }

        [Fact]
        public async Task Handle_ConferenceIsOpen_CreateRoomInDatabase()
        {
            // arrange
            SetConferenceIsOpen();

            var handler = Create();

            // act
            var createRoomInfo = new RoomCreationInfo("test");
            var request = new CreateRoomsRequest(ConferenceId, new[] {createRoomInfo});

            await handler.Handle(request, CancellationToken.None);

            // assert
            _roomRepo.Verify(x => x.CreateRoom(ConferenceId, It.IsAny<Room>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ConferenceIsOpenAndCreateManyRooms_CreateRoomsInDatabase()
        {
            // arrange
            SetConferenceIsOpen();

            var handler = Create();

            // act
            var createRoomInfo1 = new RoomCreationInfo("test");
            var createRoomInfo2 = new RoomCreationInfo("test2");
            var request = new CreateRoomsRequest(ConferenceId, new[] {createRoomInfo1, createRoomInfo2});

            await handler.Handle(request, CancellationToken.None);

            // assert
            _roomRepo.Verify(x => x.CreateRoom(ConferenceId, It.IsAny<Room>()), Times.Exactly(2));
            _roomRepo.VerifyNoOtherCalls();
        }
    }
}
