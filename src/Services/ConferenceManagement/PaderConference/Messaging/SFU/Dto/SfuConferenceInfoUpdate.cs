using System.Collections.Generic;

namespace PaderConference.Messaging.SFU.Dto
{
    public record SfuConferenceInfoUpdate(IReadOnlyDictionary<string, string> ParticipantToRoom,
        IReadOnlyDictionary<string, SfuParticipantPermissions> Permissions,
        IReadOnlyList<string> RemovedParticipants) : SfuConferenceInfo(ParticipantToRoom, Permissions);
}
