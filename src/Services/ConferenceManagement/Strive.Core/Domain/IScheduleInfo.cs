using System;

namespace Strive.Core.Domain
{
    public interface IScheduleInfo
    {
        DateTimeOffset? StartTime { get; }

        string? ScheduleCron { get; }
    }
}
