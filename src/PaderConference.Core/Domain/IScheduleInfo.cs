using System;

namespace PaderConference.Core.Domain
{
    public interface IScheduleInfo
    {
        DateTimeOffset? StartTime { get; }

        string? ScheduleCron { get; }
    }
}
