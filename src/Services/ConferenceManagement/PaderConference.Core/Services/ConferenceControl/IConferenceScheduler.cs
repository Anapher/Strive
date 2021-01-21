using System;
using PaderConference.Core.Domain;

namespace PaderConference.Core.Services.ConferenceControl
{
    public interface IConferenceScheduler
    {
        DateTimeOffset? GetNextExecution(IScheduleInfo config);
    }
}
