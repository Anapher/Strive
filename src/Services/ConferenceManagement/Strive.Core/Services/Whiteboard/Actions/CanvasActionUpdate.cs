using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.JsonPatch;
using Strive.Core.Services.Whiteboard.CanvasData;

namespace Strive.Core.Services.Whiteboard.Actions
{
    public record CanvasActionUpdate(IReadOnlyList<CanvasObjectPatch> Patches, string ParticipantId) : CanvasAction(
        ParticipantId)
    {
        public override WhiteboardCanvasUpdate Execute(WhiteboardCanvas canvas, ICanvasActionUtils utils)
        {
            var restoreObjects = new List<CanvasObjectPatch>();
            var updatedObjects = canvas.Objects.ToList();

            foreach (var patch in Patches)
            {
                var existingObject = canvas.Objects.FirstOrDefault(x => x.Id == patch.ObjectId);
                if (existingObject == null) continue;

                var index = updatedObjects.IndexOf(existingObject);

                var original = updatedObjects[index];
                var updated = original.Data with { }; // clone

                patch.Patch.ApplyTo(updated);
                updatedObjects[index] = new StoredCanvasObject(updated, original.Id);

                var undoPatch = utils.CreatePatch(updated, original.Data);
                restoreObjects.Add(new CanvasObjectPatch(undoPatch, original.Id));
            }

            var updatedCanvas = canvas with {Objects = updatedObjects};
            var undoAction = new CanvasActionUpdate(restoreObjects, ParticipantId);

            return new WhiteboardCanvasUpdate(updatedCanvas, undoAction);
        }
    }

    public record CanvasObjectPatch(JsonPatchDocument<CanvasObject> Patch, string ObjectId);
}
