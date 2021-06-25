using System.Linq;
using Strive.Core.Services.WhiteboardService;
using Strive.Core.Services.WhiteboardService.Actions;
using Strive.Core.Services.WhiteboardService.CanvasData;
using Xunit;

namespace Strive.Core.Tests.Services.WhiteboardService.Actions
{
    public class AddCanvasActionTests
    {
        private const string ParticipantId = "123";

        private readonly CanvasLine _line = new();
        private readonly CanvasText _text = new();

        [Fact]
        public void Execute_WhiteboardEmpty_AddToCanvas()
        {
            // arrange
            var action = new AddCanvasAction(new[]
            {
                new CanvasObjectRef(new StoredCanvasObject(_line, "1"), null),
                new CanvasObjectRef(new StoredCanvasObject(_text, "2"), null),
            }, ParticipantId);

            var canvas = WhiteboardCanvas.Empty;

            // act
            var (resultCanvas, undoAction) = action.Execute(canvas, null!);

            // assert
            Assert.Equal(new[] {new StoredCanvasObject(_line, "1"), new StoredCanvasObject(_text, "2")},
                resultCanvas.Objects);

            var deleteAction = Assert.IsType<DeleteCanvasAction>(undoAction);
            Assert.Equal(ParticipantId, deleteAction.ParticipantId);
            Assert.Equal(new[] {"1", "2"}, deleteAction.ObjectIds);
        }

        [Fact]
        public void Execute_WhiteboardHasObjects_AddToCanvas()
        {
            // arrange
            var action = new AddCanvasAction(new[]
            {
                new CanvasObjectRef(new StoredCanvasObject(_line, "3"), 0),
                new CanvasObjectRef(new StoredCanvasObject(_text, "4"), null),
            }, ParticipantId);

            var canvas = WhiteboardCanvas.Empty with
            {
                Objects = new[] {new StoredCanvasObject(_line, "1"), new StoredCanvasObject(_text, "2")},
            };

            // act
            var (resultCanvas, undoAction) = action.Execute(canvas, null!);

            // assert
            Assert.Equal(new[]
            {
                "3", "1", "2", "4",
            }, resultCanvas.Objects.Select(x => x.Id));

            var deleteAction = Assert.IsType<DeleteCanvasAction>(undoAction);
            Assert.Equal(ParticipantId, deleteAction.ParticipantId);
            Assert.Equal(new[] {"3", "4"}, deleteAction.ObjectIds);
        }
    }
}
