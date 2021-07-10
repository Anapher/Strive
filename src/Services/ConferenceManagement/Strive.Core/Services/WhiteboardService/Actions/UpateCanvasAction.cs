using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.JsonPatch;
using Strive.Core.Services.WhiteboardService.CanvasData;

namespace Strive.Core.Services.WhiteboardService.Actions
{
    public record UpateCanvasAction(IReadOnlyList<CanvasObjectPatch> Patches, string ParticipantId) : CanvasAction(
        ParticipantId)
    {
        /// <summary>
        ///     Execute the update on the canvas. If no object could be updated, return null
        /// </summary>
        public override WhiteboardCanvasUpdate? Execute(WhiteboardCanvas canvas, ICanvasActionUtils utils,
            int nextVersion)
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
                updatedObjects[index] = new VersionedCanvasObject(updated, original.Id, nextVersion);

                var undoPatch = utils.CreatePatch(updated, original.Data);
                restoreObjects.Add(new CanvasObjectPatch(undoPatch, original.Id));
            }

            if (!restoreObjects.Any())
                return null;

            var updatedCanvas = canvas with {Objects = updatedObjects};
            var undoAction = new UpateCanvasAction(restoreObjects, ParticipantId);

            return new WhiteboardCanvasUpdate(updatedCanvas, undoAction);
        }
    }

    public record CanvasObjectPatch(JsonPatchDocument<CanvasObject> Patch, string ObjectId);
}
