using System;

namespace PaderConference.Services
{
    public class ConferenceSchedulerOptions
    {
        public TimeSpan DefaultConferenceLength { get; set; } = TimeSpan.FromHours(2);

        /// <summary>
        ///     The time zone the cron tasks should be scheduled
        /// </summary>
        public string CronTimezone { get; set; } = TimeZoneInfo.Local.Id; // e. g. "Middle East Standard Time"
    }
}