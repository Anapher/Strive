using Microsoft.AspNetCore.JsonPatch;
using Moq;
using Strive.Core.Services.WhiteboardService;
using Strive.Core.Services.WhiteboardService.Actions;
using Strive.Core.Services.WhiteboardService.CanvasData;
using Xunit;

namespace Strive.Core.Tests.Services.WhiteboardService.Actions
{
    public class UpdateCanvasActionTests
    {
        private const string ParticipantId = "123";
        private readonly JsonPatchDocument<CanvasObject> _undoPatch = new();
        private readonly Mock<ICanvasActionUtils> _utils = new();

        public UpdateCanvasActionTests()
        {
            _utils.Setup(x => x.CreatePatch(It.IsAny<CanvasObject>(), It.IsAny<CanvasObject>())).Returns(_undoPatch);
        }

        [Fact]
        public void Execute_ObjectExists_Patch()
        {
            // arrange
            var action = new UpateCanvasAction(new[]
            {
                new CanvasObjectPatch(new JsonPatchDocument<CanvasObject>().Add(x => x.ScaleY, 2.0), "1"),
            }, ParticipantId);

            var canvas = WhiteboardCanvas.Empty with
            {
                Objects = new[] {new StoredCanvasObject(new CanvasLine {ScaleY = 1}, "1")},
            };

            // act
            var update = action.Execute(canvas, _utils.Object);
            Assert.NotNull(update);

            var (updatedCanvas, undoAction) = update!;

            // assert
            Assert.Equal(new[] {new StoredCanvasObject(new CanvasLine {ScaleY = 2}, "1")}, updatedCanvas.Objects);

            var undoUpdate = Assert.IsType<UpateCanvasAction>(undoAction);
            Assert.Equal(new[] {new CanvasObjectPatch(_undoPatch, "1")}, undoUpdate.Patches);
        }

        [Fact]
        public void Execute_ObjectDoesNotExist_ReturnNull()
        {
            // arrange
            var action = new UpateCanvasAction(new[]
            {
                new CanvasObjectPatch(new JsonPatchDocument<CanvasObject>().Add(x => x.ScaleY, 2.0), "1"),
            }, ParticipantId);

            var canvas = WhiteboardCanvas.Empty;

            // act
            var update = action.Execute(canvas, _utils.Object);

            // assert
            Assert.Null(update);
        }
    }
}
