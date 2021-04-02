using System.Collections.Generic;
using PaderConference.Core.Services.Media.Dtos;
using PaderConference.Core.Services.Synchronization;

namespace PaderConference.Core.Services.Media
{
    public record SynchronizedMediaState(IReadOnlyDictionary<string, ParticipantStreams> Streams)
    {
        public static SynchronizedObjectId SyncObjId = new(SynchronizedObjectIds.MEDIA);
    }
}
