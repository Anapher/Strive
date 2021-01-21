using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace PaderConference.Core.Extensions
{
    //https://stackoverflow.com/a/22078975
    public static class TaskExtensions
    {
        public static ValueTask<T> ToValueTask<T>(this T value)
        {
            return new ValueTask<T>(value);
        }

        public static async Task<TResult> TimeoutAfter<TResult>(this Task<TResult> task, TimeSpan timeout)
        {
            using (var timeoutCancellationTokenSource = new CancellationTokenSource())
            {
                var completedTask = await Task.WhenAny(task, Task.Delay(timeout, timeoutCancellationTokenSource.Token));
                if (completedTask == task)
                {
                    timeoutCancellationTokenSource.Cancel();
                    return await task; // Very important in order to propagate exceptions
                }

                throw new TimeoutException("The operation has timed out.");
            }
        }

        public static async Task TimeoutAfter(this Task task, TimeSpan timeout)
        {
            using (var timeoutCancellationTokenSource = new CancellationTokenSource())
            {
                var completedTask = await Task.WhenAny(task, Task.Delay(timeout, timeoutCancellationTokenSource.Token));
                if (completedTask == task)
                {
                    timeoutCancellationTokenSource.Cancel();
                    await task; // Very important in order to propagate exceptions
                    return;
                }

                throw new TimeoutException("The operation has timed out.");
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Forget(this Task task)
        {
            //Nothing here
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Forget(this ValueTask task)
        {
            //Nothing here
        }
    }
}