using System;
using PaderConference.Core.Domain.Entities;
using PaderConference.Core.Extensions;

namespace PaderConference.Core.Services.ConferenceControl
{
    public class ConferenceScheduler
    {
        public static DateTimeOffset? GetNextExecution(ConferenceConfiguration config, DateTimeOffset now,
            TimeZoneInfo timeZone)
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
