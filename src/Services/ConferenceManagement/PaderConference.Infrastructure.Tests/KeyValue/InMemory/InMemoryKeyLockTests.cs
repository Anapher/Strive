using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using PaderConference.Infrastructure.KeyValue.InMemory;
using Xunit;

namespace PaderConference.Infrastructure.Tests.KeyValue.InMemory
{
    public class InMemoryKeyLockTests
    {
        private const string LockKey = "test";

        private readonly InMemoryKeyLock _lock = new();

        [Fact]
        public async Task Lock_NotReleased_SecondLockDoesNotComplete()
        {
            // arrange
            await _lock.Lock(LockKey, CancellationToken.None);

            // act
            var secondLock = _lock.Lock(LockKey, CancellationToken.None);

            // assert
            Assert.False(secondLock.IsCompleted);
        }

        [Fact]
        public async Task Lock_Released_SecondLockDoesComplete()
        {
            // arrange
            await _lock.Lock(LockKey, CancellationToken.None);

            // act
            _lock.Unlock(LockKey);

            var secondLock = _lock.Lock(LockKey, CancellationToken.None);

            // assert
            Assert.True(secondLock.IsCompleted);
        }

        [Fact]
        public async Task Lock_AcquiredTwoTimesThenReleasedOnce_ThirdLockDoesNotComplete()
        {
            // arrange
            await _lock.Lock(LockKey, CancellationToken.None);

            var second = _lock.Lock(LockKey, CancellationToken.None);

            // act
            _lock.Unlock(LockKey);

            var third = _lock.Lock(LockKey, CancellationToken.None);

            // assert
            Assert.True(second.IsCompleted);
            Assert.False(third.IsCompleted);
        }

        [Fact]
        public async Task Lock_TestThreadSafety()
        {
            var time = 0;

            async Task TestLock()
            {
                var random = new Random();

                for (var i = 0; i < 200; i++)
                {
                    await _lock.Lock("test", CancellationToken.None);
                    var start = Interlocked.Increment(ref time);
                    await Task.Delay(random.Next(0, 2));
                    var end = Interlocked.Increment(ref time);
                    _lock.Unlock("test");

                    Assert.Equal(start + 1, end);

                    await Task.Delay(random.Next(0, 4));
                }
            }

            var tasks = Enumerable.Range(0, 2).Select(_ => Task.Run(TestLock));
            await Task.WhenAll(tasks);
        }
    }
}
