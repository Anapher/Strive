using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PaderConference.Core.Domain.Entities;
using PaderConference.Core.Interfaces.Gateways.Repositories;
using PaderConference.Core.Interfaces.Services;
using PaderConference.Extensions;

namespace PaderConference.Services
{
    public class ConferenceScheduler : BackgroundService, IConferenceScheduler
    {
        private readonly IConferenceManager _conferenceManager;
        private readonly IConferenceRepo _conferenceRepo;
        private readonly ILogger<ConferenceScheduler> _logger;
        private readonly ConferenceSchedulerOptions _options;

        private readonly Dictionary<string, (DateTimeOffset, IConferenceScheduleInfo)> _scheduledConferences =
            new Dictionary<string, (DateTimeOffset, IConferenceScheduleInfo)>();

        private readonly object _scheduleLock = new object();
        private CancellationTokenSource _cancellationToken = new CancellationTokenSource();

        public ConferenceScheduler(IConferenceManager conferenceManager, IConferenceRepo conferenceRepo,
            IOptions<ConferenceSchedulerOptions> options,
            ILogger<ConferenceScheduler> logger)
        {
            _conferenceManager = conferenceManager;
            _conferenceRepo = conferenceRepo;
            _options = options.Value;
            _logger = logger;
        }

        public ValueTask ScheduleConference(IConferenceScheduleInfo scheduleInfo, bool startAtLeast)
        {
            return ScheduleConference(scheduleInfo, startAtLeast, true);
        }

        public ValueTask RemoveConference(string conferenceId)
        {
            lock (_scheduleLock)
            {
                if (_scheduledConferences.Remove(conferenceId)) _cancellationToken.Cancel();
            }

            return new ValueTask();
        }

        public ValueTask<DateTimeOffset?> GetNextOccurrence(string conferenceId)
        {
            if (_scheduledConferences.TryGetValue(conferenceId, out var result))
                return new ValueTask<DateTimeOffset?>(result.Item1);

            return new ValueTask<DateTimeOffset?>((DateTimeOffset?) null);
        }


        /// <param name="reschedule">
        ///     if false, this method will not force the worker to reset. Useful if this method is invoked by
        ///     the worker
        /// </param>
        private async ValueTask ScheduleConference(IConferenceScheduleInfo scheduleInfo, bool startAtLeast,
            bool reschedule)
        {
            using (_logger.BeginScope("ScheduleConference()"))
            using (_logger.BeginScope(new Dictionary<string, object>
                {{"conferenceId", scheduleInfo.ConferenceId}, {"scheduleInfo", scheduleInfo}}))
            {
                if (scheduleInfo.ScheduleCron == null && scheduleInfo.StartTime == null)
                {
                    if (startAtLeast)
                    {
                        _logger.LogDebug("Start conference immediately and don't schedule");
                        await _conferenceManager.OpenConference(scheduleInfo.ConferenceId);
                    }
                    else
                    {
                        _logger.LogDebug("Mark conference as inactive");
                        await _conferenceManager.SetConferenceState(scheduleInfo.ConferenceId,
                            ConferenceState.Inactive);
                    }

                    return;
                }

                var next = GetNextExecution(scheduleInfo);
                _logger.LogDebug("Next execution: {date}", next);

                if (next == null)
                {
                    if (startAtLeast)
                    {
                        _logger.LogDebug("Start conference immediately (startAtLeast flag) and don't schedule");
                        await _conferenceManager.OpenConference(scheduleInfo.ConferenceId);
                        return;
                    }

                    _logger.LogDebug("Mark conference as inactive");
                    await _conferenceManager.SetConferenceState(scheduleInfo.ConferenceId, ConferenceState.Inactive);
                    return;
                }

                lock (_scheduleLock)
                {
                    _scheduledConferences[scheduleInfo.ConferenceId] = (next.Value, scheduleInfo);

                    if (reschedule)
                        _cancellationToken.Cancel();
                }
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using (_logger.BeginScope("ExecuteAsync"))
            {
                // TODO: subscribe redis update, reschedule

                _logger.LogDebug("Fetch active conferences...");
                var conferences = await _conferenceRepo.GetActiveConferences();
                foreach (var conference in conferences) await ScheduleConference(conference, false, false);

                _logger.LogDebug("Conferences from database initialized");

                while (true)
                {
                    using (_cancellationToken)
                    {
                        TimeSpan waitTime;
                        IConferenceScheduleInfo? next = null;

                        lock (_scheduleLock)
                        {
                            if (!_scheduledConferences.Any())
                            {
                                _logger.LogDebug("No scheduled conferences found, wait infinite");
                                waitTime = Timeout.InfiniteTimeSpan;
                            }
                            else
                            {
                                _logger.LogDebug(
                                    "Scheduled conferences found, select next one. Conferences: {@conferences}",
                                    _scheduledConferences.Keys);

                                DateTimeOffset nextTime;
                                (nextTime, next) = _scheduledConferences.Values.OrderBy(x => x.Item1).First();

                                _logger.LogDebug("Next scheduled conference {conferenceId} select at {time}",
                                    next.ConferenceId, nextTime);

                                waitTime = nextTime - DateTimeOffset.UtcNow;
                                if (waitTime < TimeSpan.Zero)
                                    waitTime = TimeSpan.Zero; // conferences may start at the same time
                            }
                        }

                        _logger.LogDebug("Wait for {timespan} to start next conference...", waitTime);

                        try
                        {
                            await Task.Delay(waitTime, _cancellationToken.Token);
                        }
                        catch (TaskCanceledException)
                        {
                            // ignore
                        }

                        if (next != null)
                        {
                            _logger.LogDebug("Start conference {conferenceId}", next.ConferenceId);
                            await _conferenceManager.OpenConference(next.ConferenceId);
                            await ScheduleConference(next, false, false); // reschedule
                        }
                    }

                    _cancellationToken = new CancellationTokenSource();
                }
            }
        }

        private DateTimeOffset? GetNextExecution(IConferenceScheduleInfo scheduleInfo)
        {
            var now = DateTimeOffset.UtcNow;

            TimeZoneInfo timeZone;
            try
            {
                timeZone = TimeZoneInfo.FindSystemTimeZoneById(_options.CronTimezone);
            }
            catch (Exception e)
            {
                timeZone = TimeZoneInfo.Local;
                _logger.LogError(e,
                    "Error occurred on finding the time zone \"{timeZone}\". Please make sure to select a valid time zone. Local time zone ({local}) is used.",
                    _options.CronTimezone, timeZone);
            }

            return GetNextExecution(scheduleInfo, now, timeZone);
        }

        public static DateTimeOffset? GetNextExecution(IConferenceScheduleInfo scheduleInfo, DateTimeOffset now,
            TimeZoneInfo timeZone)
        {
            if (scheduleInfo.StartTime != null)
                // if the start time is in future
                if (scheduleInfo.StartTime > now)
                    return scheduleInfo.StartTime.Value;

            if (scheduleInfo.ScheduleCron != null)
                return CronYearParser.GetNextOccurrence(scheduleInfo.ScheduleCron, now, timeZone);

            // there is no next execution
            return null;
        }
    }
}