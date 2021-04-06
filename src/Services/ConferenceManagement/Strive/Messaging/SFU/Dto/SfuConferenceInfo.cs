using System.Collections.Generic;

namespace Strive.Messaging.SFU.Dto
{
    public record SfuConferenceInfo(IReadOnlyDictionary<string, string> ParticipantToRoom,
        IReadOnlyDictionary<string, SfuParticipantPermissions> ParticipantPermissions);
}
