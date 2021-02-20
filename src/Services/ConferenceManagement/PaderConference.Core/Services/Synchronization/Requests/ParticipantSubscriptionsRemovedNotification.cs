using System.Collections.Generic;
using MediatR;

namespace PaderConference.Core.Services.Synchronization.Requests
{
    public record ParticipantSubscriptionsRemovedNotification(Participant Participant,
        IReadOnlyList<string> RemovedSubscriptions) : INotification;
}
