using System.Collections.Generic;
using MediatR;

namespace Strive.Core.Services.Rooms.Notifications
{
    public record ParticipantsRoomChangedNotification (string ConferenceId,
        IReadOnlyDictionary<Participant, ParticipantRoomChangeInfo> Participants) : INotification;

    public record ParticipantRoomChangeInfo(string? SourceRoom, string? TargetRoom, bool HasLeft)
    {
        public static ParticipantRoomChangeInfo Left(string? sourceRoom)
        {
            return new(sourceRoom, null, true);
        }

        public static ParticipantRoomChangeInfo Joined(string targetRoom)
        {
            return new(null, targetRoom, false);
        }

        public static ParticipantRoomChangeInfo Switched(string? sourceRoom, string? targetRoom)
        {
            return new(sourceRoom, targetRoom, false);
        }
    }
}
