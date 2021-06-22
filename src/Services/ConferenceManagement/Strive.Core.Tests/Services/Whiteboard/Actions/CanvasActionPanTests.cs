using Strive.Core.Services.Whiteboard;
using Strive.Core.Services.Whiteboard.Actions;
using Xunit;

namespace Strive.Core.Tests.Services.Whiteboard.Actions
{
    public class CanvasActionPanTests
    {
        private const string ParticipantId = "123";

        [Fact]
        public void Execute_PanWhiteboard()
        {
            // arrange
            var action = new CanvasActionPan(4, 5, ParticipantId);
            var canvas = WhiteboardCanvas.Empty with {PanX = 1, PanY = 2};

            // act
            var (updatedCanvas, undoAction) = action.Execute(canvas, null!);

            // assert
            Assert.Equal(4, updatedCanvas.PanX);
            Assert.Equal(5, updatedCanvas.PanY);

            var undoPan = Assert.IsType<CanvasActionPan>(undoAction);
            Assert.Equal(ParticipantId, undoPan.ParticipantId);
            Assert.Equal(1, undoPan.PanX);
            Assert.Equal(2, undoPan.PanY);
        }
    }
}
