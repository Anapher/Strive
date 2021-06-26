using Strive.Core.Services.WhiteboardService;
using Strive.Core.Services.WhiteboardService.Actions;
using Xunit;

namespace Strive.Core.Tests.Services.WhiteboardService.Actions
{
    public class PanCanvasActionTests
    {
        private const string ParticipantId = "123";

        [Fact]
        public void Execute_PanWhiteboard()
        {
            // arrange
            var action = new PanCanvasAction(4, 5, ParticipantId);
            var canvas = WhiteboardCanvas.Empty with {PanX = 1, PanY = 2};

            // act
            var (updatedCanvas, undoAction) = action.Execute(canvas, null!, 1)!;

            // assert
            Assert.Equal(4, updatedCanvas.PanX);
            Assert.Equal(5, updatedCanvas.PanY);

            var undoPan = Assert.IsType<PanCanvasAction>(undoAction);
            Assert.Equal(ParticipantId, undoPan.ParticipantId);
            Assert.Equal(1, undoPan.PanX);
            Assert.Equal(2, undoPan.PanY);
        }
    }
}
