using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Strive.Core.Services.Whiteboard
{
    public record Whiteboard
        (string Id, string FriendlyName, bool EveryoneCanEdit, WhiteboardCanvas Canvas) : WhiteboardInfo(FriendlyName,
            EveryoneCanEdit);

    public record WhiteboardCanvas(IReadOnlyList<JToken> UndoList, IReadOnlyList<JToken> RedoList,
        IReadOnlyList<JToken> Objects)
    {
        public static WhiteboardCanvas Empty =>
            new(Array.Empty<JToken>(), Array.Empty<JToken>(), Array.Empty<JToken>());
    }
}
