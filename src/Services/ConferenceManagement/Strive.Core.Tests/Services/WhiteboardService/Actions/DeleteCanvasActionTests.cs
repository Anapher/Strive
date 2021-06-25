using Strive.Core.Services.WhiteboardService;
using Strive.Core.Services.WhiteboardService.Actions;
using Strive.Core.Services.WhiteboardService.CanvasData;
using Xunit;

namespace Strive.Core.Tests.Services.WhiteboardService.Actions
{
    public class DeleteCanvasActionTests
    {
        private const string ParticipantId = "123";

        private readonly CanvasLine _line = new();
        private readonly CanvasText _text = new();

        [Fact]
        public void Execute_WhiteboardEmpty_ReturnNull()
        {
            // arrange
            var action = new DeleteCanvasAction(new[] {"1", "2"}, ParticipantId);
            var canvas = WhiteboardCanvas.Empty;

            // act
            var result = action.Execute(canvas, null!);

            // assert
            Assert.Null(result);
        }

        [Fact]
        public void Execute_WhiteboardHasElements_Delete()
        {
            // arrange
            var action = new DeleteCanvasAction(new[] {"1"}, ParticipantId);
            var canvas = WhiteboardCanvas.Empty with
            {
                Objects = new[] {new StoredCanvasObject(_line, "1"), new StoredCanvasObject(_text, "2")},
            };

            // act
            var update = action.Execute(canvas, null!);
            Assert.NotNull(update);

            var (updatedCanvas, undoAction) = update!;

            // assert
            Assert.Equal(new[] {new StoredCanvasObject(_text, "2")}, updatedCanvas.Objects);
            Assert.Equal(ParticipantId, undoAction.ParticipantId);

            var undoAdd = Assert.IsType<AddCanvasAction>(undoAction);
            Assert.Equal(new[] {new CanvasObjectRef(new StoredCanvasObject(_line, "1"), 0)}, undoAdd.Objects);
        }
    }
}
