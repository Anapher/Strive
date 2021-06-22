using System.Collections.Generic;
using Strive.Core.Services.Synchronization;

namespace Strive.Core.Services.Whiteboard
{
    public record SynchronizedWhiteboards(IReadOnlyDictionary<string, SynchronizedWhiteboardInfo> Whiteboards)
    {
        public const string ROOM_ID = "roomId";

        public static SynchronizedObjectId SyncObjId(string roomId)
        {
            return new(SynchronizedObjectIds.WHITEBOARDS, new Dictionary<string, string> {{ROOM_ID, roomId}});
        }
    }
}
