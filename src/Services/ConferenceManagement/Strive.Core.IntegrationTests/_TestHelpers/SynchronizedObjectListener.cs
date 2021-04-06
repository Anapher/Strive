using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Strive.Core.Services;
using Strive.Core.Services.Synchronization;
using Strive.Core.Services.Synchronization.Notifications;

namespace Strive.Core.IntegrationTests._TestHelpers
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

        public T GetSynchronizedObject<T>(Participant participant, string syncObjId)
        {
            foreach (var notification in _notifications.AsEnumerable().Reverse())
            {
                if (notification.SyncObjId == syncObjId && notification.Participants.Contains(participant))
                    return (T) notification.Value;
            }

            throw new InvalidOperationException("The synchronized object does not exist");
        }

        public T GetSynchronizedObject<T>(Participant participant, SynchronizedObjectId syncObjId)
        {
            return GetSynchronizedObject<T>(participant, syncObjId.ToString());
        }
    }
}
