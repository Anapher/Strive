using System.Collections.Generic;
using MediatR;

namespace PaderConference.Core.Services.Rooms.Notifications
{
    public record ParticipantsRoomChangedNotification (string ConferenceId, IReadOnlyList<Participant> Participants,
        bool ParticipantsLeft) : INotification;
}
