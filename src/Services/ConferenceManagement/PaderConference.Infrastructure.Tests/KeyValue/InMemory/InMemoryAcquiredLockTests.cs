using System;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using Nito.AsyncEx;
using PaderConference.Infrastructure.KeyValue.InMemory;
using Xunit;

namespace PaderConference.Infrastructure.Tests.KeyValue.InMemory
{
    public class InMemoryAcquiredLockTests
    {
        private const string TestLockKey = "test";

        private static readonly TimeSpan DefaultExpiry = TimeSpan.FromSeconds(30);


        [Fact]
        public async Task DisposeAsync_Acquired_Unlock()
        {
            // arrange
            var memoryLock = new Mock<IInMemoryKeyLock>();

            var acquired = new InMemoryAcquiredLock(TestLockKey, memoryLock.Object, DefaultExpiry);

            // act
            await acquired.DisposeAsync();

            // assert
            memoryLock.Verify(x => x.Unlock(TestLockKey), Times.Once);
        }

        [Fact]
        public async Task DisposeAsync_ExecuteTwice_UnlockOnce()
        {
            // arrange
            var memoryLock = new Mock<IInMemoryKeyLock>();

            var acquired = new InMemoryAcquiredLock(TestLockKey, memoryLock.Object, DefaultExpiry);

            // act
            await acquired.DisposeAsync();
            await acquired.DisposeAsync();

            // assert
            memoryLock.Verify(x => x.Unlock(TestLockKey), Times.Once);
        }

        [Fact]
        public async Task DisposeAsync_Expiry_UnlockAndHandleLostTokenSet()
        {
            // arrange
            var memoryLock = new Mock<IInMemoryKeyLock>();

            var acquired = new InMemoryAcquiredLock(TestLockKey, memoryLock.Object, TimeSpan.FromMilliseconds(10));

            await Assert.ThrowsAsync<TaskCanceledException>(async () =>
                await Task.Delay(TimeSpan.FromMinutes(2), acquired.HandleLostToken));

            // assert
            memoryLock.Verify(x => x.Unlock(TestLockKey), Times.Once);
        }

        [Fact]
        public async Task DisposeAsync_ThreadSafety()
        {
            // arrange
            var readerWriterLock = new AsyncReaderWriterLock();
            var writerLock = readerWriterLock.WriterLock();

            var finished = false;

            var memoryLock = new Mock<IInMemoryKeyLock>();
            var acquired = new InMemoryAcquiredLock(TestLockKey, memoryLock.Object, DefaultExpiry);

            async Task TestLock()
            {
                while (!finished)
                    using (await readerWriterLock.ReaderLockAsync())
                    {
                        await acquired.DisposeAsync();
                    }
            }

            var tasks = Enumerable.Range(0, 3).Select(_ => Task.Run(TestLock)).ToList();
            for (var i = 0; i < 50; i++)
            {
                writerLock.Dispose();
                await Task.Delay(1);
                writerLock = readerWriterLock.WriterLock();

                memoryLock.Verify(x => x.Unlock(TestLockKey), Times.Once);

                memoryLock = new Mock<IInMemoryKeyLock>();
                acquired = new InMemoryAcquiredLock(TestLockKey, memoryLock.Object, DefaultExpiry);
            }

            // assert
            finished = true;
            writerLock.Dispose();
        }
    }
}
