using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Moq;
using Strive.Core.Services;
using Strive.Core.Services.WhiteboardService;
using Strive.Core.Services.WhiteboardService.Actions;
using Strive.Core.Services.WhiteboardService.CanvasData;
using Strive.Core.Services.WhiteboardService.Requests;
using Strive.Core.Services.WhiteboardService.UseCases;
using Strive.Tests.Utils;
using Xunit;

namespace Strive.Core.Tests.Services.WhiteboardService.UseCases
{
    public class PushActionUseCaseTests : ActionRequestTestBase
    {
        private readonly Mock<IMediator> _mediator = new();
        private readonly Mock<ICanvasActionUtils> _actionUtils = new();

        private readonly CanvasAction _addAction = new CanvasActionAdd(
            new[] {new CanvasObjectRef(new StoredCanvasObject(new CanvasLine(), "2345"), null)}, ParticipantId);

        private PushActionUseCase Create()
        {
            return new(_mediator.Object, _actionUtils.Object);
        }

        [Fact]
        public async Task Handle_EmptyUpdate_ThrowException()
        {
            // arrange
            var useCase = Create();
            var capturedRequest = _mediator.CaptureRequest<UpdateWhiteboardRequest, Unit>();

            await useCase.Handle(
                new PushActionRequest(ConferenceId, RoomId, WhiteboardId,
                    new CanvasActionDelete(new[] {"1", "2"}, ParticipantId)), CancellationToken.None);

            // act
            var error = Assert.Throws<IdErrorException>(() => Execute(capturedRequest,
                CreateWhiteboard(WhiteboardCanvas.Empty, ImmutableDictionary<string, ParticipantWhiteboardState>.Empty,
                    1)));

            // assert
            Assert.Equal(WhiteboardError.WhiteboardActionHadNoEffect.Code, error.Error.Code);
        }

        [Fact]
        public async Task Handle_ValidAction_AddToUndo()
        {
            // arrange
            var useCase = Create();
            var capturedRequest = _mediator.CaptureRequest<UpdateWhiteboardRequest, Unit>();

            await useCase.Handle(new PushActionRequest(ConferenceId, RoomId, WhiteboardId, _addAction),
                CancellationToken.None);

            // act
            var updatedWhiteboard = Execute(capturedRequest,
                CreateWhiteboard(WhiteboardCanvas.Empty, ImmutableDictionary<string, ParticipantWhiteboardState>.Empty,
                    56));

            // assert
            var undoAction = Assert.Single(updatedWhiteboard.ParticipantStates[ParticipantId].UndoList);
            Assert.IsType<CanvasActionDelete>(undoAction.Action);
            Assert.Equal(56, undoAction.Version);
        }

        [Fact]
        public async Task Handle_ValidAction_UpdateWhiteboard()
        {
            // arrange
            var useCase = Create();
            var capturedRequest = _mediator.CaptureRequest<UpdateWhiteboardRequest, Unit>();

            await useCase.Handle(new PushActionRequest(ConferenceId, RoomId, WhiteboardId, _addAction),
                CancellationToken.None);

            // act
            var updatedWhiteboard = Execute(capturedRequest,
                CreateWhiteboard(WhiteboardCanvas.Empty, ImmutableDictionary<string, ParticipantWhiteboardState>.Empty,
                    56));

            // assert
            Assert.Single(updatedWhiteboard.Canvas.Objects);
        }

        [Fact]
        public async Task Handle_ValidAction_ClearRedo()
        {
            // arrange
            var useCase = Create();
            var capturedRequest = _mediator.CaptureRequest<UpdateWhiteboardRequest, Unit>();

            await useCase.Handle(new PushActionRequest(ConferenceId, RoomId, WhiteboardId, _addAction),
                CancellationToken.None);

            // act
            var updatedWhiteboard = Execute(capturedRequest, CreateWhiteboard(WhiteboardCanvas.Empty,
                new Dictionary<string, ParticipantWhiteboardState>
                {
                    {
                        ParticipantId,
                        new ParticipantWhiteboardState(ImmutableList<VersionedAction>.Empty,
                            new[] {new VersionedAction(_addAction, 45)}.ToImmutableList())
                    },
                }, 56));

            // assert
            Assert.Empty(updatedWhiteboard.ParticipantStates[ParticipantId].RedoList);
        }
    }
}
