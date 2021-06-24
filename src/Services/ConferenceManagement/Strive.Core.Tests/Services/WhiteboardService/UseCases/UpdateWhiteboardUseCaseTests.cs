using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Options;
using Moq;
using Strive.Core.Services;
using Strive.Core.Services.Rooms;
using Strive.Core.Services.Synchronization.Requests;
using Strive.Core.Services.WhiteboardService;
using Strive.Core.Services.WhiteboardService.Actions;
using Strive.Core.Services.WhiteboardService.Gateways;
using Strive.Core.Services.WhiteboardService.Requests;
using Strive.Core.Services.WhiteboardService.UseCases;
using Strive.Core.Tests._TestHelpers;
using Strive.Infrastructure.KeyValue.Abstractions;
using Strive.Tests.Utils;
using Xunit;

namespace Strive.Core.Tests.Services.WhiteboardService.UseCases
{
    public class UpdateWhiteboardUseCaseTests
    {
        private const string ConferenceId = "123";
        private const string RoomId = "room1";
        private const string WhiteboardId = "DaVinci";

        private readonly Mock<IMediator> _mediator = new();
        private readonly Mock<IWhiteboardRepository> _repository = new();
        private readonly Mock<IAcquiredLock> _lock = new();
        private bool _lockAcquired;

        private readonly WhiteboardOptions _options = new() {MaxUndoHistory = 2, MaxUndoHistoryForParticipant = 1};

        private readonly CanvasAction _randomAction = new CanvasActionDelete(new[] {"1"}, "45");

        private UpdateWhiteboardUseCase Create()
        {
            _repository.Setup(x => x.LockWhiteboard(ConferenceId, RoomId, WhiteboardId))
                .Callback(() => _lockAcquired = true).ReturnsAsync(_lock.Object);
            return new(_mediator.Object, _repository.Object, new OptionsWrapper<WhiteboardOptions>(_options));
        }

        private void VerifyLockAcquired()
        {
            _repository.Setup(x => x.Get(ConferenceId, RoomId, WhiteboardId))
                .Callback(() => Assert.True(_lockAcquired));
        }

        private void SetupWhiteboard(Whiteboard whiteboard)
        {
            _repository.Setup(x => x.Get(ConferenceId, RoomId, WhiteboardId)).ReturnsAsync(whiteboard);
        }

        private Whiteboard CreateWhiteboard(WhiteboardCanvas canvas,
            IReadOnlyDictionary<string, ParticipantWhiteboardState> states, int version)
        {
            return new(WhiteboardId, "Da Vinci", false, canvas, states.ToImmutableDictionary(), version);
        }

        private void SetupRooms(params Room[] rooms)
        {
            _mediator.SetupSyncObj(SynchronizedRooms.SyncObjId,
                new SynchronizedRooms(rooms, "1", ImmutableDictionary<string, string>.Empty));
        }

        private ObjectWrapper<Whiteboard> ListenForWhiteboard()
        {
            var wrapper = new ObjectWrapper<Whiteboard>();

            _repository
                .Setup(x => x.Create(ConferenceId, RoomId, It.IsAny<Whiteboard>()))
                .Callback((string _, string _, Whiteboard whiteboard) =>
                {
                    wrapper.Object = whiteboard;
                });

            return wrapper;
        }

        [Fact]
        public async Task Handle_WhiteboardDoesNotExist_ThrowException()
        {
            // arrange
            VerifyLockAcquired();
            var useCase = Create();

            // act
            var exception = await Assert.ThrowsAsync<IdErrorException>(async () =>
                await useCase.Handle(
                    new UpdateWhiteboardRequest(ConferenceId, RoomId, WhiteboardId, whiteboard => whiteboard),
                    CancellationToken.None));

            Assert.Equal(WhiteboardError.WhiteboardNotFound.Code, exception.Error.Code);
        }

        [Fact]
        public async Task Handle_RoomDoesNotExist_DeleteAndThrowException()
        {
            // arrange
            VerifyLockAcquired();
            var whiteboard = CreateWhiteboard(WhiteboardCanvas.Empty,
                ImmutableDictionary<string, ParticipantWhiteboardState>.Empty, 1);
            SetupWhiteboard(whiteboard);
            SetupRooms();

            var useCase = Create();

            // act
            var exception = await Assert.ThrowsAsync<IdErrorException>(async () =>
                await useCase.Handle(
                    new UpdateWhiteboardRequest(ConferenceId, RoomId, WhiteboardId, whiteboard => whiteboard),
                    CancellationToken.None));

            Assert.Equal(ConferenceError.RoomNotFound.Code, exception.Error.Code);

            _repository.Verify(x => x.Delete(ConferenceId, RoomId, WhiteboardId), Times.Once);
        }

        [Fact]
        public async Task Handle_EverythingOk_IncrementVersion()
        {
            // arrange
            var whiteboard = CreateWhiteboard(WhiteboardCanvas.Empty,
                ImmutableDictionary<string, ParticipantWhiteboardState>.Empty, 1);
            SetupWhiteboard(whiteboard);
            SetupRooms(new Room(RoomId, "room"));

            var createdWhiteboard = ListenForWhiteboard();

            var useCase = Create();

            // act
            await useCase.Handle(new UpdateWhiteboardRequest(ConferenceId, RoomId, WhiteboardId, x => x),
                CancellationToken.None);

            // assert
            Assert.NotNull(createdWhiteboard.Object);
            Assert.Equal(2, createdWhiteboard.Object!.Version);
        }

        [Fact]
        public async Task Handle_EverythingOk_ReplaceWhiteboard()
        {
            // arrange
            var whiteboard = CreateWhiteboard(WhiteboardCanvas.Empty,
                ImmutableDictionary<string, ParticipantWhiteboardState>.Empty, 1);
            SetupWhiteboard(whiteboard);
            SetupRooms(new Room(RoomId, "room"));

            var createdWhiteboard = ListenForWhiteboard();

            var useCase = Create();

            var replacement = new Whiteboard(WhiteboardId, "Wtf", false,
                WhiteboardCanvas.Empty, ImmutableDictionary<string, ParticipantWhiteboardState>.Empty, 5);

            // act
            await useCase.Handle(new UpdateWhiteboardRequest(ConferenceId, RoomId, WhiteboardId, x => replacement),
                CancellationToken.None);

            // assert
            Assert.Equal(replacement with {Version = 2}, createdWhiteboard.Object);
        }

        [Fact]
        public async Task Handle_EverythingOk_UpdateSyncObjects()
        {
            // arrange
            var whiteboard = CreateWhiteboard(WhiteboardCanvas.Empty,
                ImmutableDictionary<string, ParticipantWhiteboardState>.Empty, 1);
            SetupWhiteboard(whiteboard);
            SetupRooms(new Room(RoomId, "room"));

            var request = _mediator.CaptureRequest<UpdateSynchronizedObjectRequest, Unit>();

            var useCase = Create();

            // act
            await useCase.Handle(new UpdateWhiteboardRequest(ConferenceId, RoomId, WhiteboardId, x => x),
                CancellationToken.None);

            // assert
            Assert.Equal(ConferenceId, request.GetRequest().ConferenceId);
            Assert.Equal(SynchronizedWhiteboards.SyncObjId(RoomId).ToString(),
                request.GetRequest().SynchronizedObjectId.ToString());
        }

        private IImmutableList<VersionedAction> GetActions(int[] versions)
        {
            return versions.Select(x => new VersionedAction(_randomAction, x)).OrderBy(x => x.Version)
                .ToImmutableList();
        }

        [Fact]
        public async Task Handle_ParticipantUndoLimitExceeded_TruncateUndoItems()
        {
            // arrange
            var whiteboard = CreateWhiteboard(WhiteboardCanvas.Empty,
                ImmutableDictionary<string, ParticipantWhiteboardState>.Empty, 1);
            SetupWhiteboard(whiteboard);
            SetupRooms(new Room(RoomId, "room"));

            var createdWhiteboardWrapper = ListenForWhiteboard();

            var state = new ParticipantWhiteboardState(GetActions(new[] {0, 1, 2}),
                ImmutableList<VersionedAction>.Empty);

            var replacement = CreateWhiteboard(WhiteboardCanvas.Empty,
                new Dictionary<string, ParticipantWhiteboardState> {{"1", state}}, 1);

            var useCase = Create();

            // act
            await useCase.Handle(new UpdateWhiteboardRequest(ConferenceId, RoomId, WhiteboardId, x => replacement),
                CancellationToken.None);

            // assert
            Assert.NotNull(createdWhiteboardWrapper.Object);
            var createdWhiteboard = createdWhiteboardWrapper.Object!;

            Assert.Equal(new[] {2}, GetParticipantStateUndoVersions(createdWhiteboard.ParticipantStates["1"]));
        }

        [Fact]
        public async Task Handle_ParticipantUndoLimitExceeded_ClearRedo()
        {
            // arrange
            var whiteboard = CreateWhiteboard(WhiteboardCanvas.Empty,
                ImmutableDictionary<string, ParticipantWhiteboardState>.Empty, 1);
            SetupWhiteboard(whiteboard);
            SetupRooms(new Room(RoomId, "room"));

            _options.MaxUndoHistoryForParticipant = 2;

            var createdWhiteboardWrapper = ListenForWhiteboard();

            var state = new ParticipantWhiteboardState(GetActions(new[] {0, 1}), GetActions(new[] {2, 3}));

            var replacement = CreateWhiteboard(WhiteboardCanvas.Empty,
                new Dictionary<string, ParticipantWhiteboardState> {{"1", state}}, 1);

            var useCase = Create();

            // act
            await useCase.Handle(new UpdateWhiteboardRequest(ConferenceId, RoomId, WhiteboardId, x => replacement),
                CancellationToken.None);

            // assert
            Assert.NotNull(createdWhiteboardWrapper.Object);
            var createdWhiteboard = createdWhiteboardWrapper.Object!;

            Assert.Equal(new[] {0, 1}, GetParticipantStateUndoVersions(createdWhiteboard.ParticipantStates["1"]));
            Assert.Empty(createdWhiteboard.ParticipantStates["1"].RedoList);
        }

        private static IEnumerable<int> GetParticipantStateUndoVersions(ParticipantWhiteboardState state)
        {
            return state.UndoList.Select(x => x.Version);
        }

        [Fact]
        public async Task Handle_GlobalUndoLimitExceeded_TruncateUndoItems()
        {
            // arrange
            var whiteboard = CreateWhiteboard(WhiteboardCanvas.Empty,
                ImmutableDictionary<string, ParticipantWhiteboardState>.Empty, 1);
            SetupWhiteboard(whiteboard);
            SetupRooms(new Room(RoomId, "room"));

            var createdWhiteboardWrapper = ListenForWhiteboard();

            var state1 = new ParticipantWhiteboardState(GetActions(new[] {0}), ImmutableList<VersionedAction>.Empty);
            var state2 = new ParticipantWhiteboardState(GetActions(new[] {1}), ImmutableList<VersionedAction>.Empty);
            var state3 = new ParticipantWhiteboardState(GetActions(new[] {2}), ImmutableList<VersionedAction>.Empty);

            var replacement = CreateWhiteboard(WhiteboardCanvas.Empty,
                new Dictionary<string, ParticipantWhiteboardState> {{"1", state1}, {"2", state2}, {"3", state3}}, 1);

            var useCase = Create();

            // act
            await useCase.Handle(new UpdateWhiteboardRequest(ConferenceId, RoomId, WhiteboardId, x => replacement),
                CancellationToken.None);

            // assert
            Assert.NotNull(createdWhiteboardWrapper.Object);
            var createdWhiteboard = createdWhiteboardWrapper.Object !;

            Assert.Empty(createdWhiteboard.ParticipantStates["1"].UndoList);
            Assert.Equal(new[] {1}, GetParticipantStateUndoVersions(createdWhiteboard.ParticipantStates["2"]));
            Assert.Equal(new[] {2}, GetParticipantStateUndoVersions(createdWhiteboard.ParticipantStates["3"]));
        }

        [Fact]
        public async Task Handle_GlobalUndoLimitExceeded_TruncateRedoItems()
        {
            // arrange
            var whiteboard = CreateWhiteboard(WhiteboardCanvas.Empty,
                ImmutableDictionary<string, ParticipantWhiteboardState>.Empty, 1);
            SetupWhiteboard(whiteboard);
            SetupRooms(new Room(RoomId, "room"));

            var createdWhiteboardWrapper = ListenForWhiteboard();

            var state1 = new ParticipantWhiteboardState(ImmutableList<VersionedAction>.Empty, GetActions(new[] {0}));
            var state2 = new ParticipantWhiteboardState(GetActions(new[] {1}), ImmutableList<VersionedAction>.Empty);
            var state3 = new ParticipantWhiteboardState(GetActions(new[] {2}), ImmutableList<VersionedAction>.Empty);

            var replacement = CreateWhiteboard(WhiteboardCanvas.Empty,
                new Dictionary<string, ParticipantWhiteboardState> {{"1", state1}, {"2", state2}, {"3", state3}}, 1);

            var useCase = Create();

            // act
            await useCase.Handle(new UpdateWhiteboardRequest(ConferenceId, RoomId, WhiteboardId, x => replacement),
                CancellationToken.None);

            // assert
            Assert.NotNull(createdWhiteboardWrapper.Object);
            var createdWhiteboard = createdWhiteboardWrapper.Object!;

            Assert.Empty(createdWhiteboard.ParticipantStates["1"].UndoList);
            Assert.Empty(createdWhiteboard.ParticipantStates["1"].RedoList);
            Assert.Equal(new[] {1}, GetParticipantStateUndoVersions(createdWhiteboard.ParticipantStates["2"]));
            Assert.Equal(new[] {2}, GetParticipantStateUndoVersions(createdWhiteboard.ParticipantStates["3"]));
        }

        private class ObjectWrapper<T>
        {
            public T? Object { get; set; }
        }
    }
}
