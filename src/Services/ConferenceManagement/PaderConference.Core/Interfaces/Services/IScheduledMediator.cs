using System;
using System.Threading.Tasks;

namespace PaderConference.Core.Interfaces.Services
{
    public interface IScheduledMediator
    {
        ValueTask<string> Schedule<T>(T notification, DateTimeOffset scheduleDate)
            where T : class, IScheduledNotification;

        ValueTask Remove<T>(string id) where T : class, IScheduledNotification;
    }
}
