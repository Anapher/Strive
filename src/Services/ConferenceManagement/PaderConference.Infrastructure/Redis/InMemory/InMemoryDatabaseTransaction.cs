using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Nito.Disposables;
using PaderConference.Infrastructure.Redis.Abstractions;
using PaderConference.Infrastructure.Redis.Scripts;
using StackExchange.Redis;

namespace PaderConference.Infrastructure.Redis.InMemory
{
    public class InMemoryDatabaseTransaction : InMemoryDatabaseActions, IKeyValueDatabaseTransaction
    {
        private readonly InMemoryDatabaseData _data;
        private readonly List<Func<ValueTask>> _transactionSteps = new();
        private readonly CancellationTokenSource _cancellationTokenSource = new();
        private bool _isExecuting;

        public InMemoryDatabaseTransaction(InMemoryDatabaseData data) : base(data.Data)
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

            using (_data.Lock.Lock())
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

        public override ValueTask<string?> HashGetAsync(string hashKey, string key)
        {
            return AddTransactionStep(() => base.HashGetAsync(hashKey, key));
        }

        public override ValueTask HashSetAsync(string hashKey, IEnumerable<KeyValuePair<string, string>> keyValuePairs)
        {
            return AddTransactionStep(() => base.HashSetAsync(hashKey, keyValuePairs));
        }

        public override ValueTask HashSetAsync(string hashKey, string key, string value)
        {
            return AddTransactionStep(() => base.HashSetAsync(hashKey, key, value));
        }

        public override ValueTask<bool> HashExists(string hashKey, string key)
        {
            return AddTransactionStep(() => base.HashExists(hashKey, key));
        }

        public override ValueTask<bool> HashDeleteAsync(string hashKey, string key)
        {
            return AddTransactionStep(() => base.HashDeleteAsync(hashKey, key));
        }

        public override ValueTask<IReadOnlyDictionary<string, string>> HashGetAllAsync(string hashKey)
        {
            return AddTransactionStep(() => base.HashGetAllAsync(hashKey));
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

        public override ValueTask<RedisResult> ExecuteScriptAsync(RedisScript script, params object[] parameters)
        {
            return AddTransactionStep(() => base.ExecuteScriptAsync(script, parameters));
        }
    }
}
