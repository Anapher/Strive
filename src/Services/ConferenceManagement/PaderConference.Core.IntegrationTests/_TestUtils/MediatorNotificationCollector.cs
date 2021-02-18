using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Xunit;

namespace PaderConference.Core.IntegrationTests._TestUtils
{
    public class MediatorNotificationCollector : INotificationHandler<INotification>
    {
        private readonly List<INotification> _notifications = new();
        private readonly object _notificationsLock = new();

        Task INotificationHandler<INotification>.Handle(INotification notification, CancellationToken cancellationToken)
        {
            lock (_notificationsLock)
            {
                _notifications.Add(notification);
            }

            return Task.CompletedTask;
        }

        public void Reset()
        {
            lock (_notificationsLock)
            {
                _notifications.Clear();
            }
        }

        public void AssertSingleNotificationIssued<T>(Action<T>? assertNotificationFunc = null) where T : INotification
        {
            var notification = _notifications.OfType<T>().Single();
            _notifications.Remove(notification);

            assertNotificationFunc?.Invoke(notification);
        }

        public void AssertNoMoreNotifications()
        {
            Assert.Empty(_notifications);
        }
    }
}
