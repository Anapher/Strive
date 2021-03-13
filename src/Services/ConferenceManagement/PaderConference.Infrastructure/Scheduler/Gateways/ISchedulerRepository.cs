using System;
using System.Threading.Tasks;

namespace PaderConference.Infrastructure.Scheduler.Gateways
{
    public interface ISchedulerRepository
    {
        ValueTask<Guid?> SetScheduledNotification(string notificationId, Guid tokenId);

        ValueTask Remove(string notificationId);
    }
}
