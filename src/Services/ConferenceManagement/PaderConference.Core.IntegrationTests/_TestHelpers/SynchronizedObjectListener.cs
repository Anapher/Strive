using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PaderConference.Core.Services;
using PaderConference.Core.Services.Synchronization;
using PaderConference.Core.Services.Synchronization.Notifications;

namespace PaderConference.Core.IntegrationTests._TestHelpers
{
    public class SynchronizedObjectListener : INotificationHandler<SynchronizedObjectUpdatedNotification>
    {
        private readonly List<SynchronizedObjectUpdatedNotification> _notifications = new();

        Task INotificationHandler<SynchronizedObjectUpdatedNotification>.Handle(
            SynchronizedObjectUpdatedNotification notification, CancellationToken cancellationToken)
        {
            _notifications.Add(notification);
            return Task.CompletedTask;
        }

        public T? GetSynchronizedObject<T>(Participant participant, string syncObjId)
        {
            foreach (var notification in _notifications.AsEnumerable().Reverse())
            {
                if (notification.SyncObjId == syncObjId && notification.Participants.Contains(participant))
                    return (T) notification.Value;
            }

            return default;
        }

        public T? GetSynchronizedObject<T>(Participant participant, SynchronizedObjectId syncObjId)
        {
            return GetSynchronizedObject<T>(participant, syncObjId.ToString());
        }
    }
}
