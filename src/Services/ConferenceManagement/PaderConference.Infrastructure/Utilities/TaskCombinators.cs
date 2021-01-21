using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PaderConference.Infrastructure.Utilities
{
    /// <summary>
    ///     Contains task execution strategies, such as parallel throttled execution.
    /// </summary>
    public static class TaskCombinators
    {
        public const int MaxDegreeOfParallelism = 16;

        public static async Task<IEnumerable<TValue>> ThrottledAsync<TSource, TValue>(IEnumerable<TSource> sources,
            Func<TSource, CancellationToken, ValueTask<TValue>> valueSelector, CancellationToken cancellationToken)
        {
            var values = new ConcurrentQueue<TValue>();

            async Task TaskBody(IEnumerator<TSource> enumerator)
            {
                using (enumerator)
                    while (enumerator.MoveNext())
                    {
                        cancellationToken.ThrowIfCancellationRequested();

                        var value = await valueSelector(enumerator.Current, cancellationToken);
                        values.Enqueue(value);
                    }
            }

            var tasks = Partitioner.Create(sources).GetPartitions(MaxDegreeOfParallelism)
                .Select(enumerator => Task.Run(() => TaskBody(enumerator)));
            await Task.WhenAll(tasks);

            return values;
        }

        public static async Task<IEnumerable<TValue>> ThrottledIgnoreErrorsAsync<TSource, TValue>(IEnumerable<TSource> sources,
            Func<TSource, CancellationToken, Task<TValue>> valueSelector, CancellationToken cancellationToken)
        {
            var values = new ConcurrentQueue<TValue>();

            async Task TaskBody(IEnumerator<TSource> enumerator)
            {
                using (enumerator)
                    while (enumerator.MoveNext())
                    {
                        cancellationToken.ThrowIfCancellationRequested();

                        try
                        {
                            var value = await valueSelector(enumerator.Current, cancellationToken);
                            values.Enqueue(value);
                        }
                        catch (Exception)
                        {
                            //ignored
                        }
                    }
            }

            var tasks = Partitioner.Create(sources).GetPartitions(MaxDegreeOfParallelism)
                .Select(enumerator => Task.Run(() => TaskBody(enumerator)));
            await Task.WhenAll(tasks);

            return values;
        }

        public static async Task<IEnumerable<TItem>> ThrottledFilterItems<TItem>(IEnumerable<TItem> sources,
            Func<TItem, CancellationToken, Task<bool>> valueSelector, CancellationToken cancellationToken)
        {
            var values = new ConcurrentQueue<TItem>();

            async Task TaskBody(IEnumerator<TItem> enumerator)
            {
                using (enumerator)
                    while (enumerator.MoveNext())
                    {
                        cancellationToken.ThrowIfCancellationRequested();

                        var result = await valueSelector(enumerator.Current, cancellationToken);

                        if (result)
                            values.Enqueue(enumerator.Current);
                    }
            }

            var tasks = Partitioner.Create(sources).GetPartitions(MaxDegreeOfParallelism)
                .Select(enumerator => Task.Run(() => TaskBody(enumerator)));
            await Task.WhenAll(tasks);

            return values;
        }

        public static async Task ThrottledAsync<TSource>(IEnumerable<TSource> sources,
            Func<TSource, CancellationToken, Task> valueSelector, CancellationToken cancellationToken)
        {
            async Task TaskBody(IEnumerator<TSource> enumerator)
            {
                using (enumerator)
                    while (enumerator.MoveNext())
                    {
                        cancellationToken.ThrowIfCancellationRequested();

                        await valueSelector(enumerator.Current, cancellationToken);
                    }
            }

            var tasks = Partitioner.Create(sources).GetPartitions(MaxDegreeOfParallelism)
                .Select(enumerator => Task.Run(() => TaskBody(enumerator)));
            await Task.WhenAll(tasks);
        }

        public static async Task<IReadOnlyDictionary<TSource, Exception>> ThrottledCatchErrorsAsync<TSource>(
            IEnumerable<TSource> sources, Func<TSource, CancellationToken, Task> valueSelector,
            CancellationToken cancellationToken) where TSource:notnull
        {
            var exceptions = new ConcurrentDictionary<TSource, Exception>();

            async Task TaskBody(IEnumerator<TSource> enumerator)
            {
                using (enumerator)
                    while (enumerator.MoveNext())
                    {
                        cancellationToken.ThrowIfCancellationRequested();

                        try
                        {
                            await valueSelector(enumerator.Current, cancellationToken);
                        }
                        catch (OperationCanceledException)
                        {
                            throw;
                        }
                        catch (Exception e)
                        {
                            exceptions.TryAdd(enumerator.Current, e);
                        }
                    }
            }

            var tasks = Partitioner.Create(sources).GetPartitions(MaxDegreeOfParallelism)
                .Select(enumerator => Task.Run(() => TaskBody(enumerator)));
            await Task.WhenAll(tasks);

            return exceptions;
        }

        public static IDictionary<string, Task<TValue>> ObserveErrorsAsync<TSource, TValue>(
            IEnumerable<TSource> sources, Func<TSource, string> keySelector,
            Func<TSource, CancellationToken, Task<TValue>> valueSelector, Action<Task, object?> observeErrorAction,
            CancellationToken cancellationToken)
        {
            var tasks = sources.ToDictionary(keySelector, s =>
            {
                var valueTask = valueSelector(s, cancellationToken);
                var ignored = valueTask.ContinueWith(observeErrorAction, s, cancellationToken,
                    TaskContinuationOptions.OnlyOnFaulted, TaskScheduler.Current);
                return valueTask;
            });

            return tasks;
        }
    }
}