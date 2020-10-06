using System.Collections.Concurrent;
using System.Threading;
using Microsoft.MixedReality.WebRTC;

namespace PaderConference.Hubs.Media
{
    public class I420AVideoFrameContainer
    {
        private int _lockCounter;

        public uint Width { get; set; }

        public uint Height { get; set; }

        public int StrideY { get; set; }

        public int StrideU { get; set; }

        public int StrideV { get; set; }

        public int StrideA { get; set; }

        public byte[]? Buffer { get; set; }

        public bool IsLocked => _lockCounter > 0;

        public bool Lock()
        {
            return Interlocked.Increment(ref _lockCounter) > 0;
        }

        public void Unlock()
        {
            Interlocked.Decrement(ref _lockCounter);
        }

        public bool TryDispose()
        {
            return Interlocked.CompareExchange(ref _lockCounter, int.MinValue, 0) == int.MinValue;
        }
    }

    public class AsyncVideoFrameQueue
    {
        public delegate void FrameUsageAction(I420AVideoFrame frame);

        /// <summary>
        ///     Queue of frames pending delivery to sink.
        /// </summary>
        private readonly ConcurrentQueue<I420AVideoFrameContainer> _frameQueue =
            new ConcurrentQueue<I420AVideoFrameContainer>();

        private readonly int _maxQueueLength;
        private I420AVideoFrameContainer? _currentFrame;

        public AsyncVideoFrameQueue(int maxQueueLength)
        {
            _maxQueueLength = maxQueueLength;
        }

        /// <summary>
        ///     Enqueue a new video frame encoded in I420+Alpha format.
        ///     If the internal queue reached its maximum capacity, do nothing and drop the frame.
        /// </summary>
        /// <param name="frame">The video frame to enqueue</param>
        /// <returns>Return <c>true</c> if the frame was enqueued successfully, or <c>false</c> if it was dropped</returns>
        /// <remarks>This should only be used if the queue has storage for a compatible video frame encoding.</remarks>
        public void Enqueue(I420AVideoFrame frame)
        {
            I420AVideoFrameContainer? container = null;

            for (var i = 0; i < _frameQueue.Count; i++)
                if (_frameQueue.TryDequeue(out container))
                    if (!container.TryDispose())
                        _frameQueue.Enqueue(container);

            container ??= new I420AVideoFrameContainer();

            // Try to get some storage for that new frame
            var byteSize = (frame.strideY + frame.strideA) * frame.height +
                           (frame.strideU + frame.strideV) * frame.height / 2;

            if (container.Buffer != null)
                if (container.Buffer.Length < byteSize)
                    container.Buffer = null;

            if (container.Buffer == null)
                container.Buffer = new byte[byteSize];

            // Copy the new frame to its storage
            frame.CopyTo(container.Buffer);

            container.StrideA = frame.strideA;
            container.StrideU = frame.strideU;
            container.StrideV = frame.strideV;
            container.StrideY = frame.strideY;
            container.Width = frame.width;
            container.Height = frame.height;

            if (_currentFrame != null)
                _frameQueue.Enqueue(_currentFrame);

            _currentFrame = container;
        }

        public unsafe void UseCurrentFrame(FrameUsageAction action)
        {
            if (_currentFrame == null) return;

            var frame = _currentFrame;
            if (!frame.Lock())
            {
                UseCurrentFrame(action);
                return;
            }

            try
            {
                fixed (byte* ptr = frame.Buffer)
                {
                }
            }
            finally
            {
                frame.Unlock();
            }
        }

        /// <summary>
        ///     Get some video frame storage for a frame of the given byte size.
        /// </summary>
        /// <param name="byteSize">The byte size of the frame that the storage should accomodate</param>
        /// <returns>A new or recycled storage if possible, or <c>null</c> if the queue reached its maximum capacity</returns>
        private T GetStorageFor(ulong byteSize)
        {
            if (_unusedFramePool.TryPop(out T storage))
            {
                if (storage.Capacity < byteSize) storage.Capacity = byteSize;
                return storage;
            }

            if (_frameQueue.Count >= _maxQueueLength)
                // Too many frames in queue, drop the current one
                return null;
            var newStorage = new T();
            newStorage.Capacity = byteSize;
            return newStorage;
        }
    }
}