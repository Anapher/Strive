using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Xunit;

namespace PaderConference.Tests.Utils
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

        public void AssertSingleNotificationIssued<T>(Func<T, bool> filter, Action<T>? assertNotificationFunc = null)
            where T : INotification
        {
            var notification = _notifications.OfType<T>().Where(filter).Single();
            _notifications.Remove(notification);

            assertNotificationFunc?.Invoke(notification);
        }

        public void AssertLastNotificationIssued<T>(Action<T>? assertNotificationFunc = null) where T : INotification
        {
            var notification = _notifications.OfType<T>().Reverse().First();
            _notifications.Remove(notification);

            assertNotificationFunc?.Invoke(notification);
        }

        public void AssertNoNotificationOfType<T>() where T : INotification
        {
            var hasNotification = _notifications.OfType<T>().Any();
            Assert.False(hasNotification);
        }

        public void AssertNoMoreNotifications()
        {
            Assert.Empty(_notifications);
        }

        public IReadOnlyList<T> GetNotificationsIssued<T>() where T : INotification
        {
            return _notifications.OfType<T>().ToList();
        }
    }
}
