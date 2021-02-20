using System.Collections.Immutable;
using MediatR;

namespace PaderConference.Core.Services.Synchronization.Notifications
{
    public record SynchronizedObjectUpdatedNotification(IImmutableList<Participant> Participants, string SyncObjId,
        object Value, object? PreviousValue) : INotification;
}
