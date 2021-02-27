using System;
using System.Threading;
using System.Threading.Tasks;

namespace PaderConference.Core.Services
{
    public interface ITaskDelay
    {
        Task Delay(TimeSpan delay, CancellationToken cancellationToken = default);
    }

    public class TaskDelay : ITaskDelay
    {
        public Task Delay(TimeSpan delay, CancellationToken cancellationToken = default)
        {
            return Task.Delay(delay, cancellationToken);
        }
    }
}
