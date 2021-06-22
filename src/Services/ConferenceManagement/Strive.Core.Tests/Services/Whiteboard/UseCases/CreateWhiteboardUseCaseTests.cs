using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Moq;
using Strive.Core.Services;
using Strive.Core.Services.Rooms;
using Strive.Core.Services.Synchronization.Requests;
using Strive.Core.Services.Whiteboard.Gateways;
using Strive.Core.Services.Whiteboard.Requests;
using Strive.Core.Services.Whiteboard.UseCases;
using Strive.Core.Tests._TestHelpers;
using Strive.Tests.Utils;
using Xunit;

namespace Strive.Core.Tests.Services.Whiteboard.UseCases
{
    public class CreateWhiteboardUseCaseTests
    {
        private const string ConferenceId = "123";
        private const string RoomId = "54";

        private readonly Mock<IWhiteboardRepository> _repository = new();
        private readonly Mock<IMediator> _mediator = new();

        private CreateWhiteboardUseCase Create()
        {
            return new(_repository.Object, _mediator.Object);
        }

        private void SetupRooms(params Room[] rooms)
        {
            _mediator.SetupSyncObj(SynchronizedRooms.SyncObjId,
                new SynchronizedRooms(rooms, "1", ImmutableDictionary<string, string>.Empty));
        }

        [Fact]
        public async Task Handle_RoomDoesNotExist_DeleteAndThrow()
        {
            // arrange
            var useCase = Create();
            SetupRooms();

            // act
            await Assert.ThrowsAsync<IdErrorException>(async () =>
                await useCase.Handle(new CreateWhiteboardRequest(ConferenceId, RoomId), CancellationToken.None));

            // assert
            _repository.Verify(x => x.Delete(ConferenceId, RoomId, It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task Handle_RoomDoesExist_CreateInRepository()
        {
            // arrange
            var useCase = Create();
            SetupRooms(new Room(RoomId, "test"));

            // act
            await useCase.Handle(new CreateWhiteboardRequest(ConferenceId, RoomId), CancellationToken.None);

            // assert
            _repository.Verify(x => x.Create(ConferenceId, RoomId, It.IsAny<Core.Services.Whiteboard.Whiteboard>()),
                Times.Once);
            _repository.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task Handle_RoomDoesExist_UpdateSyncObj()
        {
            // arrange
            var captured = _mediator.CaptureRequest<UpdateSynchronizedObjectRequest, Unit>();

            var useCase = Create();
            SetupRooms(new Room(RoomId, "test"));

            // act
            await useCase.Handle(new CreateWhiteboardRequest(ConferenceId, RoomId), CancellationToken.None);

            // assert
            captured.AssertReceived();
        }
    }
}
