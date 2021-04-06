using System.Collections.Generic;
using MediatR;

namespace Strive.Core.Services.Synchronization.Notifications
{
    public record SynchronizedObjectUpdatedNotification(IReadOnlyList<Participant> Participants, string SyncObjId,
        object Value, object? PreviousValue) : INotification;
}
