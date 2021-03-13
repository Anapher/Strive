using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Options;
using Moq;
using PaderConference.Core.Extensions;
using PaderConference.Core.Interfaces.Gateways.Repositories;
using PaderConference.Core.Interfaces.Services;
using PaderConference.Core.Services;
using PaderConference.Core.Services.BreakoutRooms;
using PaderConference.Core.Services.BreakoutRooms.Gateways;
using PaderConference.Core.Services.BreakoutRooms.Internal;
using PaderConference.Core.Services.BreakoutRooms.Notifications;
using PaderConference.Core.Services.Rooms;
using PaderConference.Core.Services.Rooms.Requests;
using PaderConference.Core.Services.Synchronization.Requests;
using PaderConference.Tests.Utils;
using Xunit;

namespace PaderConference.Core.Tests.Services.BreakoutRooms.Internal
{
    public class ApplyBreakoutRoomUseCaseTests
    {
        private const string ConferenceId = "test";

        private readonly Mock<IBreakoutRoomsRepository> _repository = new();
        private readonly Mock<IMediator> _mediator = new();
        private readonly Mock<IScheduledMediator> _scheduledMediator = new();
        private readonly Mock<IAsyncDisposable> _lock = new();

        private readonly BreakoutRoomInternalState _sampleInternalState =
            new(new ActiveBreakoutRoomState(1, null, null), new[] {"test"}, null);

        private BreakoutRoomInternalState? _capturedState;

        private ApplyBreakoutRoomUseCase Create()
        {
            SetupLock(_lock.Object);
            SetupRoomsCreatedWithIndexAsId();

            return new ApplyBreakoutRoomUseCase(_repository.Object, _mediator.Object, _scheduledMediator.Object,
                new OptionsWrapper<BreakoutRoomsOptions>(new BreakoutRoomsOptions()));
        }

        private void SetupInternalState(BreakoutRoomInternalState? state)
        {
            _repository.Setup(x => x.Get(ConferenceId)).ReturnsAsync(state);
        }

        private void SetupLock(IAsyncDisposable lockDisposable)
        {
            _repository.Setup(x => x.LockBreakoutRooms(ConferenceId)).ReturnsAsync(lockDisposable);
        }

        private void CaptureNewInternalState()
        {
            _repository.Setup(x => x.Set(ConferenceId, It.IsAny<BreakoutRoomInternalState>()))
                .Callback((string _, BreakoutRoomInternalState state) => _capturedState = state);
        }

        private void VerifyRoomsCreated(int amount)
        {
            _mediator.Verify(
                x => x.Send(It.Is<CreateRoomsRequest>(request => request.Rooms.Count == amount),
                    It.IsAny<CancellationToken>()), Times.Once);
        }

        private void VerifyRoomsRemoved(IEnumerable<string> removedRooms)
        {
            _mediator.Verify(
                x => x.Send(It.Is<RemoveRoomsRequest>(request => request.RoomIds.ScrambledEquals(removedRooms)),
                    It.IsAny<CancellationToken>()), Times.Once);
        }

        private void VerifySyncObjUpdated()
        {
            _mediator.Verify(
                x => x.Send(
                    It.Is<UpdateSynchronizedObjectRequest>(x =>
                        x.ConferenceId == ConferenceId &&
                        x.SynchronizedObjectId.Id == SynchronizedObjectIds.BREAKOUT_ROOMS),
                    It.IsAny<CancellationToken>()), Times.Once);
        }

        private void SetupRoomsCreatedWithIndexAsId()
        {
            _mediator.Setup(x => x.Send(It.IsAny<CreateRoomsRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync(
                (CreateRoomsRequest request, CancellationToken _) =>
                {
                    var counter = 0;
                    return request.Rooms.Select(x => new Room(counter++.ToString(), x.DisplayName)).ToList();
                });
        }

        private void SetupTimerTokenForDeadline(DateTimeOffset deadline, string token)
        {
            _scheduledMediator.Setup(x => x.Schedule(It.IsAny<BreakoutRoomTimerElapsedNotification>(), deadline))
                .ReturnsAsync(token);
        }

        private void SetupSynchronizedRooms(SynchronizedRooms syncRooms)
        {
            _mediator.Setup(x => x.Send(It.IsAny<FetchSynchronizedObjectRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(syncRooms);
        }

        [Fact]
        public async Task Handle_ApplyState_ReleaseSuppliedLock()
        {
            // arrange
            var lockMock = new Mock<IAsyncDisposable>();

            SetupInternalState(null);

            var useCase = Create();
            var request = new ApplyBreakoutRoomRequest(ConferenceId, null, lockMock.Object);

            // act
            await useCase.Handle(request, CancellationToken.None);

            // assert
            lockMock.Verify(x => x.DisposeAsync(), Times.Once);
            _repository.Verify(x => x.LockBreakoutRooms(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ApplyState_ReleaseAcquiredLock()
        {
            // arrange
            SetupInternalState(null);

            var useCase = Create();
            var request = new ApplyBreakoutRoomRequest(ConferenceId, null);

            // act
            await useCase.Handle(request, CancellationToken.None);

            // assert
            _lock.Verify(x => x.DisposeAsync(), Times.Once);
        }

        [Fact]
        public async Task Handle_CreateNewAndAlreadyOpened_ThrowConcurrencyException()
        {
            // arrange
            SetupInternalState(_sampleInternalState);

            var useCase = Create();
            var request = new ApplyBreakoutRoomRequest(ConferenceId, null, null, true);

            // act
            await Assert.ThrowsAsync<ConcurrencyException>(async () =>
                await useCase.Handle(request, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_RemoveStateAndInternalStateExists_RemoveTimerRemoveRoomsRemoveStateUpdateSyncObj()
        {
            // arrange
            var activeState = new ActiveBreakoutRoomState(2, DateTimeOffset.MinValue, "description");
            var currentState = new BreakoutRoomInternalState(activeState, new[] {"test1", "test2"}, "123");

            var capturedRequest = _mediator.CaptureRequest<RemoveRoomsRequest, Unit>();

            SetupInternalState(currentState);

            var useCase = Create();
            var request = new ApplyBreakoutRoomRequest(ConferenceId, null);

            // act
            await useCase.Handle(request, CancellationToken.None);

            // assert
            _scheduledMediator.Verify(x => x.Remove<BreakoutRoomTimerElapsedNotification>(currentState.TimerTokenId!),
                Times.Once);

            var removeRoomsRequest = capturedRequest.GetRequest();
            Assert.Equal(ConferenceId, removeRoomsRequest.ConferenceId);
            Assert.Equal(currentState.OpenedRooms, removeRoomsRequest.RoomIds);

            _repository.Verify(x => x.Remove(ConferenceId), Times.Once);

            VerifySyncObjUpdated();
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task Handle_CreateNewState_SetStateInRepository(bool createNew)
        {
            const string timerToken = "123";

            // arrange
            SetupInternalState(null);
            CaptureNewInternalState();

            var newState = new ActiveBreakoutRoomState(2, DateTimeOffset.MinValue, "description");
            SetupTimerTokenForDeadline(newState.Deadline!.Value, timerToken);

            var useCase = Create();
            var request = new ApplyBreakoutRoomRequest(ConferenceId, newState, null, createNew);

            // act
            await useCase.Handle(request, CancellationToken.None);

            // assert
            Assert.NotNull(_capturedState);
            Assert.Equal(2, _capturedState!.OpenedRooms.Count);
            Assert.Equal(timerToken, _capturedState!.TimerTokenId);
            Assert.Equal(newState, _capturedState.State);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task Handle_CreateNewStateWithDeadline_CreateTimer(bool createNew)
        {
            const string timerToken = "123";

            // arrange
            SetupInternalState(null);

            var newState = new ActiveBreakoutRoomState(2, DateTimeOffset.MinValue, "description");
            SetupTimerTokenForDeadline(newState.Deadline!.Value, timerToken);

            var useCase = Create();
            var request = new ApplyBreakoutRoomRequest(ConferenceId, newState, null, createNew);

            // act
            await useCase.Handle(request, CancellationToken.None);

            // assert
            _scheduledMediator.Verify(
                x => x.Schedule(It.IsAny<BreakoutRoomTimerElapsedNotification>(), newState.Deadline!.Value),
                Times.Once);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task Handle_CreateNewState_CreateRooms(bool createNew)
        {
            // arrange
            SetupInternalState(null);
            CaptureNewInternalState();

            var newState = new ActiveBreakoutRoomState(2, null, "description");

            var useCase = Create();
            var request = new ApplyBreakoutRoomRequest(ConferenceId, newState, null, createNew);

            // act
            await useCase.Handle(request, CancellationToken.None);

            // assert
            VerifyRoomsCreated(2);
            VerifySyncObjUpdated();
        }

        [Fact]
        public async Task Handle_PatchStateNewDeadline_ChangeTimer()
        {
            const string currentTimerToken = "123";

            // arrange
            var activeState = new ActiveBreakoutRoomState(2, DateTimeOffset.MinValue, "description");
            var currentState = new BreakoutRoomInternalState(activeState, new[] {"test1", "test2"}, "123");

            SetupInternalState(currentState);

            var newState = activeState with {Deadline = DateTimeOffset.MaxValue};

            var useCase = Create();
            var request = new ApplyBreakoutRoomRequest(ConferenceId, newState);

            // act
            await useCase.Handle(request, CancellationToken.None);

            // assert
            _scheduledMediator.Verify(x => x.Remove<BreakoutRoomTimerElapsedNotification>(currentTimerToken),
                Times.Once);
            _scheduledMediator.Verify(
                x => x.Schedule(It.IsAny<BreakoutRoomTimerElapsedNotification>(), newState.Deadline.Value), Times.Once);

            VerifySyncObjUpdated();
        }

        [Fact]
        public async Task Handle_PatchStateRemoveDeadline_ChangeTimer()
        {
            const string currentTimerToken = "123";

            // arrange
            var activeState = new ActiveBreakoutRoomState(2, DateTimeOffset.MinValue, "description");
            var currentState = new BreakoutRoomInternalState(activeState, new[] {"test1", "test2"}, "123");

            SetupInternalState(currentState);

            var newState = activeState with {Deadline = null};

            var useCase = Create();
            var request = new ApplyBreakoutRoomRequest(ConferenceId, newState);

            // act
            await useCase.Handle(request, CancellationToken.None);

            // assert
            _scheduledMediator.Verify(x => x.Remove<BreakoutRoomTimerElapsedNotification>(currentTimerToken),
                Times.Once);
            _scheduledMediator.VerifyNoOtherCalls();

            VerifySyncObjUpdated();
        }

        [Fact]
        public async Task Handle_PatchStateCreateDeadline_ChangeTimer()
        {
            // arrange
            var activeState = new ActiveBreakoutRoomState(2, null, "description");
            var currentState = new BreakoutRoomInternalState(activeState, new[] {"test1", "test2"}, null);

            SetupInternalState(currentState);

            var newState = activeState with {Deadline = DateTimeOffset.MinValue};

            var useCase = Create();
            var request = new ApplyBreakoutRoomRequest(ConferenceId, newState);

            // act
            await useCase.Handle(request, CancellationToken.None);

            // assert
            _scheduledMediator.Verify(
                x => x.Schedule(It.IsAny<BreakoutRoomTimerElapsedNotification>(), newState.Deadline.Value), Times.Once);
            _scheduledMediator.VerifyNoOtherCalls();

            VerifySyncObjUpdated();
        }

        [Fact]
        public async Task Handle_PatchStateChangeDeadline_SetNewTimerToken()
        {
            const string timerToken = "1234";

            // arrange
            var activeState = new ActiveBreakoutRoomState(2, null, "description");
            var currentState = new BreakoutRoomInternalState(activeState, new[] {"test1", "test2"}, null);

            SetupInternalState(currentState);
            CaptureNewInternalState();

            var newState = activeState with {Deadline = DateTimeOffset.MinValue};
            SetupTimerTokenForDeadline(newState.Deadline.Value, timerToken);

            var useCase = Create();
            var request = new ApplyBreakoutRoomRequest(ConferenceId, newState);

            // act
            await useCase.Handle(request, CancellationToken.None);

            // assert
            Assert.Equal(timerToken, _capturedState?.TimerTokenId);
        }

        [Fact]
        public async Task Handle_ChangeDescription_SetNewInternalState()
        {
            const string newDescription = "giga yikes";

            // arrange
            var activeState = new ActiveBreakoutRoomState(2, null, "description");
            var currentState = new BreakoutRoomInternalState(activeState, new[] {"test1", "test2"}, null);

            SetupInternalState(currentState);
            CaptureNewInternalState();

            var newState = activeState with {Description = newDescription};

            var useCase = Create();
            var request = new ApplyBreakoutRoomRequest(ConferenceId, newState);

            // act
            await useCase.Handle(request, CancellationToken.None);

            // assert
            Assert.Equal(newDescription, _capturedState?.State.Description);
            VerifySyncObjUpdated();
        }

        private async Task ChangeRooms(IReadOnlyList<Room> rooms, IEnumerable<string> roomOccupancy, int newAmount)
        {
            // arrange
            var counter = 0;
            SetupSynchronizedRooms(new SynchronizedRooms(rooms, "main",
                roomOccupancy.ToDictionary(_ => counter++.ToString(), x => x)));

            var activeState = new ActiveBreakoutRoomState(rooms.Count, null, null);
            var currentState = new BreakoutRoomInternalState(activeState, rooms.Select(x => x.RoomId).ToList(), null);

            SetupInternalState(currentState);
            CaptureNewInternalState();

            var newState = activeState with {Amount = newAmount};

            var useCase = Create();
            var request = new ApplyBreakoutRoomRequest(ConferenceId, newState);

            // act
            await useCase.Handle(request, CancellationToken.None);

            // assert
            Assert.Equal(newAmount, _capturedState?.OpenedRooms.Count);
            VerifySyncObjUpdated();
        }

        [Fact]
        public async Task Handle_ChangeRoomAmountNoParticipants_RemoveLastRooms()
        {
            // arrange
            var existingRooms = new[]
            {
                new Room("room1", "Alpha"), new Room("room2", "Bravo"), new Room("room3", "Charlie"),
                new Room("room4", "Delta"),
            };

            var roomOccupancy = Enumerable.Empty<string>();
            var newAmount = 2;

            // act
            await ChangeRooms(existingRooms, roomOccupancy, newAmount);

            // assert
            var expectedRoomsRemoved = new[] {"room3", "room4"};
            VerifyRoomsRemoved(expectedRoomsRemoved);
        }

        [Fact]
        public async Task Handle_ChangeRoomAmountHasParticipants_RemoveRoomsWithoutParticipants()
        {
            // arrange
            var existingRooms = new[]
            {
                new Room("room1", "Alpha"), new Room("room2", "Bravo"), new Room("room3", "Charlie"),
                new Room("room4", "Delta"),
            };

            var roomOccupancy = new[] {"room3", "room4"};
            var newAmount = 2;

            // act
            await ChangeRooms(existingRooms, roomOccupancy, newAmount);

            // assert
            var expectedRoomsRemoved = new[] {"room1", "room2"};
            VerifyRoomsRemoved(expectedRoomsRemoved);
        }

        [Fact]
        public async Task Handle_ChangeRoomAmountHasParticipants_RemoveRoomsWithLeastAmountOfParticipants()
        {
            // arrange
            var existingRooms = new[]
            {
                new Room("room1", "Alpha"), new Room("room2", "Bravo"), new Room("room3", "Charlie"),
                new Room("room4", "Delta"),
            };

            var roomOccupancy = new[] {"room3", "room4", "room1", "room2", "room2", "room4"};
            var newAmount = 2;

            // act
            await ChangeRooms(existingRooms, roomOccupancy, newAmount);

            // assert
            var expectedRoomsRemoved = new[] {"room3", "room1"};
            VerifyRoomsRemoved(expectedRoomsRemoved);
        }

        [Fact]
        public async Task Handle_ChangeRoomAmount_CreateRooms()
        {
            // arrange
            var existingRooms = new[] {new Room("room1", "Alpha")};

            var roomOccupancy = Enumerable.Empty<string>();
            var newAmount = 2;

            // act
            await ChangeRooms(existingRooms, roomOccupancy, newAmount);

            // assert
            VerifyRoomsCreated(1);
        }
    }
}
