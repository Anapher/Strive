using System.Collections.Generic;
using System.Linq;

namespace Strive.Core.Services.WhiteboardService.Actions
{
    public record AddCanvasAction(IReadOnlyList<CanvasObjectRef> Objects, string ParticipantId) : CanvasAction(
        ParticipantId)
    {
        public override WhiteboardCanvasUpdate? Execute(WhiteboardCanvas canvas, ICanvasActionUtils utils,
            int nextVersion)
        {
            var objects = canvas.Objects.ToList();

            foreach (var objectRef in Objects.Where(x => x.Index != null).OrderBy(x => x.Index))
            {
                var obj = objectRef.Object;
                objects.Insert(objectRef.Index!.Value, new VersionedCanvasObject(obj.Data, obj.Id, nextVersion));
            }

            foreach (var objectRef in Objects.Where(x => x.Index == null))
            {
                var obj = objectRef.Object;
                objects.Add(new VersionedCanvasObject(obj.Data, obj.Id, nextVersion));
            }

            var updatedCanvas = canvas with {Objects = objects};
            var undoAction = new DeleteCanvasAction(Objects.Select(x => x.Object.Id).ToList(), ParticipantId);

            return new WhiteboardCanvasUpdate(updatedCanvas, undoAction);
        }
    }
}
