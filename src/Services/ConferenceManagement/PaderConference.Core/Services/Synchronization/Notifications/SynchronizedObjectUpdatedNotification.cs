using System.Collections.Generic;
using MediatR;

namespace PaderConference.Core.Services.Synchronization.Notifications
{
    public record SynchronizedObjectUpdatedNotification(IReadOnlyList<Participant> Participants, string SyncObjId,
        object Value, object? PreviousValue) : INotification;
}
