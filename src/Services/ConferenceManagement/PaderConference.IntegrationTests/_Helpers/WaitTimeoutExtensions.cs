using System;
using System.Threading;
using System.Threading.Tasks;
using Nito.AsyncEx;
using PaderConference.Core.Extensions;

namespace PaderConference.IntegrationTests._Helpers
{
    public static class WaitTimeoutExtensions
    {
        public static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(30);

        public static async Task WaitTimeoutAsync(this AsyncAutoResetEvent autoResetEvent, TimeSpan? timeout = null)
        {
            using (var source = new CancellationTokenSource(timeout ?? DefaultTimeout))
            {
                await autoResetEvent.WaitAsync(source.Token);
            }
        }

        public static Task WithDefaultTimeout(this Task task)
        {
            return task.TimeoutAfter(DefaultTimeout);
        }

        public static Task<T> WithDefaultTimeout<T>(this Task<T> task)
        {
            return task.TimeoutAfter(DefaultTimeout);
        }
    }
}
