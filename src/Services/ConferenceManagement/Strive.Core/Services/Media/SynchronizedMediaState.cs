using System.Collections.Generic;
using Strive.Core.Services.Media.Dtos;
using Strive.Core.Services.Synchronization;

namespace Strive.Core.Services.Media
{
    public record SynchronizedMediaState(IReadOnlyDictionary<string, ParticipantStreams> Streams)
    {
        public static SynchronizedObjectId SyncObjId = new(SynchronizedObjectIds.MEDIA);
    }
}
