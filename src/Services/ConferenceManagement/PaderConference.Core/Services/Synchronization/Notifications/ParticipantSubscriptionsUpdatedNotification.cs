using System.Collections.Generic;
using MediatR;

namespace PaderConference.Core.Services.Synchronization.Notifications
{
    public record ParticipantSubscriptionsUpdatedNotification(Participant Participant,
        IReadOnlyList<SynchronizedObjectId> Removed, IReadOnlyList<SynchronizedObjectId> Added) : INotification;
}
