using System.Collections.Generic;

namespace PaderConference.Messaging.SFU.Dto
{
    public record SfuConferenceInfoUpdate(IReadOnlyDictionary<string, string> ParticipantToRoom,
        IReadOnlyDictionary<string, SfuParticipantPermissions> ParticipantPermissions,
        IReadOnlyList<string> RemovedParticipants) : SfuConferenceInfo(ParticipantToRoom, ParticipantPermissions);
}
