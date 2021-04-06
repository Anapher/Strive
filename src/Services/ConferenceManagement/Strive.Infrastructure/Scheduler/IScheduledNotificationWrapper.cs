namespace Strive.Infrastructure.Scheduler
{
    public interface IScheduledNotificationWrapper
    {
        string JsonSerialized { get; }
        string TypeName { get; }
    }
}
