using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace PaderConference.Tests.Utils
{
    public static class AssertHelper
    {
        public static void AssertScrambledEquals<T>(IEnumerable<T> expected, IEnumerable<T> actual)
        {
            if (typeof(T).IsAssignableTo(typeof(IComparable)))
            {
                Assert.Equal(expected.OrderBy(x => x), actual.OrderBy(x => x));
            }
            else
            {
                var expectedList = expected.ToList();
                var actualList = actual.ToList();
                Assert.Equal(expectedList.Count, actualList.Count);

                foreach (var expectedItem in expectedList)
                {
                    Assert.Contains(expectedItem, actualList);
                }
            }
        }

        public static async Task WaitForAssert(Action assertAction, TimeSpan? timeout = null)
        {
            timeout ??= TimeSpan.FromSeconds(30);

            try
            {
                using (var cancellationTokenSource = new CancellationTokenSource(timeout.Value))
                {
                    while (true)
                    {
                        try
                        {
                            assertAction();
                            return;
                        }
                        catch (Exception)
                        {
                            // ignored
                        }

                        await Task.Delay(100, cancellationTokenSource.Token);
                    }
                }
            }
            catch (TaskCanceledException)
            {
            }

            assertAction();
        }

        public static async Task WaitForAssertAsync(Func<Task> assertAction, TimeSpan? timeout = null)
        {
            timeout ??= TimeSpan.FromSeconds(30);

            try
            {
                using (var cancellationTokenSource = new CancellationTokenSource(timeout.Value))
                {
                    while (true)
                    {
                        try
                        {
                            await assertAction();
                            return;
                        }
                        catch (Exception)
                        {
                            // ignored
                        }

                        await Task.Delay(100, cancellationTokenSource.Token);
                    }
                }
            }
            catch (TaskCanceledException)
            {
            }

            await assertAction();
        }
    }
}
