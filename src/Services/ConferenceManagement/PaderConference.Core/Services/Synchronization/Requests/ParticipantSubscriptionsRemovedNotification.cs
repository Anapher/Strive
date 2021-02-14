using System.Collections.Generic;
using MediatR;

namespace PaderConference.Core.Services.Synchronization.Requests
{
    public record ParticipantSubscriptionsRemovedNotification(string ConferenceId, string ParticipantId,
        IReadOnlyList<string> RemovedSubscriptions) : INotification;
}
