using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Nito.Disposables;
using PaderConference.Core.Extensions;
using PaderConference.Infrastructure.Redis.Abstractions;
using PaderConference.Infrastructure.Redis.Scripts;
using StackExchange.Redis;

namespace PaderConference.Infrastructure.Redis.InMemory
{
    public class InMemoryDatabaseTransaction : InMemoryDatabaseActions, IKeyValueDatabaseTransaction
    {
        private readonly InMemoryKeyValueData _data;
        private readonly List<Func<ValueTask>> _transactionSteps = new();
        private readonly CancellationTokenSource _cancellationTokenSource = new();
        private bool _isExecuting;

        public InMemoryDatabaseTransaction(InMemoryKeyValueData data) : base(data.Data)
        {
            _data = data;
        }

        public void Dispose()
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
        }

        public async ValueTask<bool> ExecuteAsync()
        {
            _cancellationTokenSource.Token.ThrowIfCancellationRequested();

            using (_data.Lock.WriterLock())
            {
                _isExecuting = true;

                foreach (var step in _transactionSteps)
                {
                    await step();
                }
            }

            return true;
        }

        private ValueTask<T> AddTransactionStep<T>(Func<ValueTask<T>> operation)
        {
            var waitHandler = new TaskCompletionSource<T>();

            async ValueTask Step()
            {
                var result = await operation();
                waitHandler.SetResult(result);
            }

            _cancellationTokenSource.Token.Register(() => waitHandler.TrySetCanceled());
            _transactionSteps.Add(Step);

            return new ValueTask<T>(waitHandler.Task);
        }

        private async ValueTask AddTransactionStep(Func<ValueTask> operation)
        {
            await AddTransactionStep(async () =>
            {
                await operation();
                return Unit.Value;
            });
        }

        protected override IDisposable Lock()
        {
            if (!_isExecuting)
                throw new InvalidOperationException(
                    "It seems like the method that is tries to lock is not wrapped in AddTransactionStep() " +
                    "(and therefor executing immediately instead of executing in the transaction). Please declare " +
                    $"the method as virtual in {nameof(InMemoryDatabaseActions)} and override it in this class");

            return NoopDisposable.Instance;
        }

        public override ValueTask<bool> KeyDeleteAsync(string key)
        {
            return AddTransactionStep(() => base.KeyDeleteAsync(key));
        }

        public override ValueTask<string?> HashGetAsync(string key, string field)
        {
            return AddTransactionStep(() => base.HashGetAsync(key, field));
        }

        public override ValueTask HashSetAsync(string key, IEnumerable<KeyValuePair<string, string>> keyValuePairs)
        {
            return AddTransactionStep(() => base.HashSetAsync(key, keyValuePairs));
        }

        public override ValueTask HashSetAsync(string key, string field, string value)
        {
            return AddTransactionStep(() =>
                base.HashSetAsync(key, new KeyValuePair<string, string>(field, value).Yield()));
        }

        public override ValueTask<bool> HashExistsAsync(string key, string field)
        {
            return AddTransactionStep(() => base.HashExistsAsync(key, field));
        }

        public override ValueTask<bool> HashDeleteAsync(string key, string field)
        {
            return AddTransactionStep(() => base.HashDeleteAsync(key, field));
        }

        public override ValueTask<IReadOnlyDictionary<string, string>> HashGetAllAsync(string key)
        {
            return AddTransactionStep(() => base.HashGetAllAsync(key));
        }

        public override ValueTask<string?> GetAsync(string key)
        {
            return AddTransactionStep(() => base.GetAsync(key));
        }

        public override ValueTask<string?> GetSetAsync(string key, string value)
        {
            return AddTransactionStep(() => base.GetSetAsync(key, value));
        }

        public override ValueTask SetAsync(string key, string value)
        {
            return AddTransactionStep(() => base.SetAsync(key, value));
        }

        public override ValueTask<RedisResult> ExecuteScriptAsync(RedisScript script, params string[] parameters)
        {
            return AddTransactionStep(() => base.ExecuteScriptAsync(script, parameters));
        }

        public override ValueTask ListRightPushAsync(string key, string item)
        {
            return AddTransactionStep(() => base.ListRightPushAsync(key, item));
        }

        public override ValueTask<int> ListLenAsync(string key)
        {
            return AddTransactionStep(() => base.ListLenAsync(key));
        }

        public override ValueTask<IReadOnlyList<string>> ListRangeAsync(string key, int start, int end)
        {
            return AddTransactionStep(() => base.ListRangeAsync(key, start, end));
        }

        public override ValueTask<bool> SetAddAsync(string key, string value)
        {
            return AddTransactionStep(() => base.SetAddAsync(key, value));
        }

        public override ValueTask<bool> SetRemoveAsync(string key, string value)
        {
            return AddTransactionStep(() => base.SetRemoveAsync(key, value));
        }

        public override ValueTask<IReadOnlyList<string>> SetMembersAsync(string key)
        {
            return AddTransactionStep(() => base.SetMembersAsync(key));
        }
    }
}
