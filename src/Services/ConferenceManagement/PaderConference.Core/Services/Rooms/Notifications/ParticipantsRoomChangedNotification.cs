using System.Collections.Generic;
using MediatR;

namespace PaderConference.Core.Services.Rooms.Notifications
{
    public record ParticipantsRoomChangedNotification
        (string ConferenceId, IEnumerable<string> Participants) : INotification;
}
