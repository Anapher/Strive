using System;
using Strive.Core.Services.Whiteboard;
using Strive.Core.Services.Whiteboard.Actions;
using Strive.Core.Services.Whiteboard.CanvasData;
using Xunit;

namespace Strive.Core.Tests.Services.Whiteboard.Actions
{
    public class CanvasActionDeleteTests
    {
        private const string ParticipantId = "123";

        private readonly CanvasLine _line = new();
        private readonly CanvasText _text = new();

        [Fact]
        public void Execute_WhiteboardEmpty_ThrowError()
        {
            // arrange
            var action = new CanvasActionDelete(new[] {"1", "2"}, ParticipantId);
            var canvas = WhiteboardCanvas.Empty;

            // act
            Assert.ThrowsAny<Exception>(() => action.Execute(canvas, null!));
        }

        [Fact]
        public void Execute_WhiteboardHasElements_Delete()
        {
            // arrange
            var action = new CanvasActionDelete(new[] {"1"}, ParticipantId);
            var canvas = WhiteboardCanvas.Empty with
            {
                Objects = new[] {new StoredCanvasObject(_line, "1"), new StoredCanvasObject(_text, "2")},
            };

            // act
            var (updatedCanvas, undoAction) = action.Execute(canvas, null!);

            // assert
            Assert.Equal(new[] {new StoredCanvasObject(_text, "2")}, updatedCanvas.Objects);
            Assert.Equal(ParticipantId, undoAction.ParticipantId);

            var undoAdd = Assert.IsType<CanvasActionAdd>(undoAction);
            Assert.Equal(new[] {new CanvasObjectRef(new StoredCanvasObject(_line, "1"), 0)}, undoAdd.Objects);
        }
    }
}
