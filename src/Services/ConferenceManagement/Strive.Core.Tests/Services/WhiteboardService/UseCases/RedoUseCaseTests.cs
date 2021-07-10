using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Moq;
using Strive.Core.Services;
using Strive.Core.Services.WhiteboardService;
using Strive.Core.Services.WhiteboardService.Actions;
using Strive.Core.Services.WhiteboardService.CanvasData;
using Strive.Core.Services.WhiteboardService.Requests;
using Strive.Core.Services.WhiteboardService.Responses;
using Strive.Core.Services.WhiteboardService.UseCases;
using Strive.Tests.Utils;
using Xunit;

namespace Strive.Core.Tests.Services.WhiteboardService.UseCases
{
    public class RedoUseCaseTests : ActionRequestTestBase
    {
        private readonly Mock<IMediator> _mediator = new();
        private readonly Mock<ICanvasActionUtils> _actionUtils = new();


        private readonly CanvasAction _addAction = new AddCanvasAction(
            new[] {new CanvasObjectRef(new StoredCanvasObject(new CanvasLine(), "2345"), null)}, ParticipantId);

        private RedoUseCase Create()
        {
            return new(_mediator.Object, _actionUtils.Object);
        }

        [Fact]
        public async Task ParticipantRedo_NoRedoAvailable_Throw()
        {
            // arrange
            var useCase = Create();
            var capturedRequest = _mediator.CaptureRequest<UpdateWhiteboardRequest, WhiteboardUpdatedResponse>();
            await useCase.Handle(new RedoRequest(ConferenceId, RoomId, WhiteboardId, ParticipantId),
                CancellationToken.None);

            // act
            var error = Assert.Throws<IdErrorException>(() => Execute(capturedRequest,
                CreateWhiteboard(WhiteboardCanvas.Empty, ImmutableDictionary<string, ParticipantWhiteboardState>.Empty,
                    1)));

            // assert
            Assert.Equal(WhiteboardError.RedoNotAvailable.Code, error.Error.Code);
        }

        [Fact]
        public async Task GlobalRedo_NoRedoAvailable_Throw()
        {
            // arrange
            var useCase = Create();
            var capturedRequest = _mediator.CaptureRequest<UpdateWhiteboardRequest, WhiteboardUpdatedResponse>();
            await useCase.Handle(new RedoRequest(ConferenceId, RoomId, WhiteboardId, null), CancellationToken.None);

            // act
            var error = Assert.Throws<IdErrorException>(() => Execute(capturedRequest,
                CreateWhiteboard(WhiteboardCanvas.Empty, ImmutableDictionary<string, ParticipantWhiteboardState>.Empty,
                    1)));

            // assert
            Assert.Equal(WhiteboardError.RedoNotAvailable.Code, error.Error.Code);
        }

        [Fact]
        public async Task ParticipantRedo_IsAvailable_ExecuteRedo()
        {
            // arrange
            var useCase = Create();
            var capturedRequest = _mediator.CaptureRequest<UpdateWhiteboardRequest, WhiteboardUpdatedResponse>();
            await useCase.Handle(new RedoRequest(ConferenceId, RoomId, WhiteboardId, null), CancellationToken.None);

            // act
            var updatedWhiteboard = Execute(capturedRequest, CreateWhiteboard(WhiteboardCanvas.Empty,
                new Dictionary<string, ParticipantWhiteboardState>
                {
                    {
                        ParticipantId,
                        new ParticipantWhiteboardState(ImmutableList<VersionedAction>.Empty, new[]
                        {
                            new VersionedAction(new PanCanvasAction(5, 5, ParticipantId), 1),
                            new VersionedAction(_addAction, 2),
                        }.ToImmutableList())
                    },
                }, 1));

            // assert
            Assert.Single(updatedWhiteboard.Canvas.Objects);
            Assert.IsType<PanCanvasAction>(Assert.Single(updatedWhiteboard.ParticipantStates[ParticipantId].RedoList)
                .Action);
            Assert.Single(updatedWhiteboard.ParticipantStates[ParticipantId].UndoList);
        }

        [Fact]
        public async Task ParticipantRedo_UndoNotEmpty_AddToEndOfUndo()
        {
            // arrange
            var useCase = Create();
            var capturedRequest = _mediator.CaptureRequest<UpdateWhiteboardRequest, WhiteboardUpdatedResponse>();
            await useCase.Handle(new RedoRequest(ConferenceId, RoomId, WhiteboardId, null), CancellationToken.None);

            // act
            var updatedWhiteboard = Execute(capturedRequest, CreateWhiteboard(WhiteboardCanvas.Empty,
                new Dictionary<string, ParticipantWhiteboardState>
                {
                    {
                        ParticipantId,
                        new ParticipantWhiteboardState(new[]
                        {
                            new VersionedAction(new PanCanvasAction(5, 5, ParticipantId), 1),
                        }.ToImmutableList(), new[]
                        {
                            new VersionedAction(_addAction, 2),
                        }.ToImmutableList())
                    },
                }, 2));

            // assert
            Assert.Equal(new[] {typeof(PanCanvasAction), typeof(DeleteCanvasAction)},
                updatedWhiteboard.ParticipantStates[ParticipantId].UndoList.Select(x => x.Action.GetType()));
        }

        [Fact]
        public async Task GlobalRedo_IsAvailable_ExecuteRedo()
        {
            // arrange
            var useCase = Create();
            var capturedRequest = _mediator.CaptureRequest<UpdateWhiteboardRequest, WhiteboardUpdatedResponse>();
            await useCase.Handle(new RedoRequest(ConferenceId, RoomId, WhiteboardId, null), CancellationToken.None);

            // act
            var updatedWhiteboard = Execute(capturedRequest, CreateWhiteboard(WhiteboardCanvas.Empty,
                new Dictionary<string, ParticipantWhiteboardState>
                {
                    {
                        "other",
                        new ParticipantWhiteboardState(ImmutableList<VersionedAction>.Empty, new[]
                        {
                            new VersionedAction(new PanCanvasAction(5, 5, ParticipantId), 2),
                            new VersionedAction(_addAction, 4),
                        }.ToImmutableList())
                    },
                    {
                        ParticipantId,
                        new ParticipantWhiteboardState(ImmutableList<VersionedAction>.Empty, new[]
                        {
                            new VersionedAction(new PanCanvasAction(5, 5, ParticipantId), 3),
                            new VersionedAction(_addAction, 5),
                        }.ToImmutableList())
                    },
                }, 1));

            // assert
            Assert.Equal(1, updatedWhiteboard.ParticipantStates[ParticipantId].UndoList.Count);
            Assert.Equal(1, updatedWhiteboard.ParticipantStates[ParticipantId].RedoList.Count);
        }
    }
}
