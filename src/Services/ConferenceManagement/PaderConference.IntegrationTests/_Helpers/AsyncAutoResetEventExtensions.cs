using System;
using System.Threading;
using System.Threading.Tasks;
using Nito.AsyncEx;

namespace PaderConference.IntegrationTests._Helpers
{
    public static class AsyncAutoResetEventExtensions
    {
        public static async Task WaitTimeoutAsync(this AsyncAutoResetEvent autoResetEvent, TimeSpan? timeout = null)
        {
            using (var source = new CancellationTokenSource(timeout ?? TimeSpan.FromSeconds(30)))
            {
                await autoResetEvent.WaitAsync(source.Token);
            }
        }
    }
}
