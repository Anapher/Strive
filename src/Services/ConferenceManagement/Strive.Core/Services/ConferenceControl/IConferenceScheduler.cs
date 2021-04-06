using System;
using Strive.Core.Domain;

namespace Strive.Core.Services.ConferenceControl
{
    public interface IConferenceScheduler
    {
        DateTimeOffset? GetNextExecution(IScheduleInfo config);
    }
}
