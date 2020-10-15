using System;
using System.Threading.Tasks;

namespace PaderConference.Core.Interfaces.Services
{
    public interface IConferenceScheduler
    {
        /// <summary>
        ///     Schedule a conference. If the conference is already scheduled, it will be updated
        /// </summary>
        /// <param name="scheduleInfo">Schedule info about the conference</param>
        /// <param name="startAtLeast">
        ///     If true, the conference will be started immediately if it cannot be scheduled, even if the
        ///     start time is in past.
        /// </param>
        ValueTask ScheduleConference(IConferenceScheduleInfo scheduleInfo, bool startAtLeast);

        /// <summary>
        ///     Remove a conference from schedule.
        /// </summary>
        /// <param name="conferenceId">The id of the conference</param>
        ValueTask RemoveConference(string conferenceId);
    }

    public interface IConferenceScheduleInfo
    {
        string ConferenceId { get; }

        DateTimeOffset? StartTime { get; }

        string? ScheduleCron { get; }
    }
}