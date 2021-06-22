using System;
using System.Collections.Generic;
using System.Linq;

namespace Strive.Core.Services.Whiteboard.Actions
{
    public record CanvasActionDelete (IReadOnlyList<string> ObjectIds, string ParticipantId) : CanvasAction(
        ParticipantId)
    {
        public override WhiteboardCanvasUpdate Execute(WhiteboardCanvas canvas, ICanvasActionUtils utils)
        {
            var whiteboardObjects = canvas.Objects.ToList();

            var deletedObjects = whiteboardObjects.Where(x => ObjectIds.Contains(x.Id))
                .Select(x => new CanvasObjectRef(x, whiteboardObjects.IndexOf(x))).ToList();

            // we ignore non existing objects if we deleted at least one object
            if (!deletedObjects.Any())
            {
                throw new InvalidOperationException("The objects were not found");
            }

            var objects = canvas.Objects.ToList();

            foreach (var deleted in deletedObjects)
            {
                objects.Remove(deleted.Object);
            }

            var updatedCanvas = canvas with {Objects = objects};
            var undoAction = new CanvasActionAdd(deletedObjects, ParticipantId);

            return new WhiteboardCanvasUpdate(updatedCanvas, undoAction);
        }
    }
}
