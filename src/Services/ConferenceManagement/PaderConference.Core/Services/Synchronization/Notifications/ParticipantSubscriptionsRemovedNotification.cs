using System.Collections.Generic;
using MediatR;

namespace PaderConference.Core.Services.Synchronization.Notifications
{
    public record ParticipantSubscriptionsRemovedNotification(Participant Participant,
        IReadOnlyList<string> RemovedSubscriptions) : INotification;
}
