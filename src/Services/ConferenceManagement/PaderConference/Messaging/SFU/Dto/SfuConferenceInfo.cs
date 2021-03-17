using System.Collections.Generic;

namespace PaderConference.Messaging.SFU.Dto
{
    public record SfuConferenceInfo(IReadOnlyDictionary<string, string> ParticipantToRoom,
        IReadOnlyDictionary<string, SfuParticipantPermissions> Permissions);
}
