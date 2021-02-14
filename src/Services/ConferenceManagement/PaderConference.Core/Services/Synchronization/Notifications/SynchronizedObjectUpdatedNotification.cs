using System.Collections.Immutable;
using MediatR;

namespace PaderConference.Core.Services.Synchronization.Notifications
{
    public record SynchronizedObjectUpdatedNotification(string ConferenceId, IImmutableList<string> ParticipantIds,
        string SyncObjId, object Value, object? PreviousValue) : INotification;
}
