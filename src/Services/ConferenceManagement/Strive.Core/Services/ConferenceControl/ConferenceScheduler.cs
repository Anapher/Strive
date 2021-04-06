using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Strive.Core.Domain;

namespace Strive.Core.Services.ConferenceControl
{
    public class ConferenceScheduler : IConferenceScheduler
    {
        private readonly IOptions<ConferenceSchedulerOptions> _options;
        private readonly ILogger<ConferenceScheduler> _logger;

        public ConferenceScheduler(IOptions<ConferenceSchedulerOptions> options, ILogger<ConferenceScheduler> logger)
        {
            _options = options;
            _logger = logger;
        }

        public DateTimeOffset? GetNextExecution(IScheduleInfo config)
        {
            var now = DateTimeOffset.UtcNow;

            TimeZoneInfo timeZone;
            try
            {
                timeZone = TimeZoneInfo.FindSystemTimeZoneById(_options.Value.CronTimezone);
            }
            catch (Exception e)
            {
                timeZone = TimeZoneInfo.Local;
                _logger.LogWarning(e,
                    "Error occurred on finding the time zone \"{timeZone}\". Please make sure to select a valid time zone. Local time zone ({local}) is used.",
                    _options.Value.CronTimezone, timeZone);
            }

            return ScheduleUtils.GetNextExecution(config, now, timeZone);
        }
    }
}
