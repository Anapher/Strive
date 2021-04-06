using System;
using Strive.Core.Domain;
using Strive.Core.Extensions;

namespace Strive.Core.Services.ConferenceControl
{
    public static class ScheduleUtils
    {
        public static DateTimeOffset? GetNextExecution(IScheduleInfo config, DateTimeOffset now, TimeZoneInfo timeZone)
        {
            if (config.StartTime != null)
                // if the start time is in future
                if (config.StartTime > now)
                    return config.StartTime.Value;

            if (config.ScheduleCron != null)
                return CronYearParser.GetNextOccurrence(config.ScheduleCron, now, timeZone);

            // there is no next execution
            return null;
        }
    }
}
