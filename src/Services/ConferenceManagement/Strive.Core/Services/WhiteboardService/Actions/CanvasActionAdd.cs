using System.Collections.Generic;
using System.Linq;

namespace Strive.Core.Services.WhiteboardService.Actions
{
    public record CanvasActionAdd(IReadOnlyList<CanvasObjectRef> Objects, string ParticipantId) : CanvasAction(
        ParticipantId)
    {
        public override WhiteboardCanvasUpdate Execute(WhiteboardCanvas canvas, ICanvasActionUtils utils)
        {
            var objects = canvas.Objects.ToList();

            foreach (var objectRef in Objects.Where(x => x.Index != null).OrderBy(x => x.Index))
            {
                objects.Insert(objectRef.Index!.Value, objectRef.Object);
            }

            foreach (var objectRef in Objects.Where(x => x.Index == null))
            {
                objects.Add(objectRef.Object);
            }

            var updatedCanvas = canvas with {Objects = objects};
            var undoAction = new CanvasActionDelete(Objects.Select(x => x.Object.Id).ToList(), ParticipantId);

            return new WhiteboardCanvasUpdate(updatedCanvas, undoAction);
        }
    }
}
