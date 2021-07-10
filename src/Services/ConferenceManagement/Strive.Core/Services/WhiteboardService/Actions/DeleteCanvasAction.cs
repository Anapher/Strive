using System.Collections.Generic;
using System.Linq;

namespace Strive.Core.Services.WhiteboardService.Actions
{
    public record DeleteCanvasAction (IReadOnlyList<string> ObjectIds, string ParticipantId) : CanvasAction(
        ParticipantId)
    {
        /// <summary>
        ///     Execute the delete action on the canvas. If no object was deleted, return null
        /// </summary>
        public override WhiteboardCanvasUpdate? Execute(WhiteboardCanvas canvas, ICanvasActionUtils utils,
            int nextVersion)
        {
            var whiteboardObjects = canvas.Objects.ToList();

            var deletedObjects = whiteboardObjects.Where(x => ObjectIds.Contains(x.Id))
                .Select(obj => (obj, whiteboardObjects.IndexOf(obj))).ToList();

            // we ignore non existing objects if we deleted at least one object
            if (!deletedObjects.Any())
                return null;

            var objects = canvas.Objects.ToList();

            foreach (var deleted in deletedObjects)
            {
                objects.Remove(deleted.obj);
            }

            var updatedCanvas = canvas with {Objects = objects};
            var undoAction =
                new AddCanvasAction(
                    deletedObjects
                        .Select(x => new CanvasObjectRef(new StoredCanvasObject(x.obj.Data, x.obj.Id), x.Item2))
                        .ToList(), ParticipantId);

            return new WhiteboardCanvasUpdate(updatedCanvas, undoAction);
        }
    }
}
