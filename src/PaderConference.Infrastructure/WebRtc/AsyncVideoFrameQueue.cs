using System;
using System.Collections.Concurrent;
using System.Threading;
using Microsoft.MixedReality.WebRTC;

namespace PaderConference.Infrastructure.WebRtc
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

        public object LockObj { get; } = new object();

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
            var disposeVal = int.MinValue / 2;
            return Interlocked.CompareExchange(ref _lockCounter, disposeVal, 0) == disposeVal;
        }
    }

    public class AsyncVideoFrameQueue
    {
        public delegate void FrameUsageAction(in I420AVideoFrame frame);

        /// <summary>
        ///     Queue of frames pending delivery to sink.
        /// </summary>
        private readonly ConcurrentQueue<I420AVideoFrameContainer> _frameQueue =
            new ConcurrentQueue<I420AVideoFrameContainer>();

        private readonly object _lock = new object();

        private I420AVideoFrameContainer? _currentFrame;

        /// <summary>
        ///     Enqueue a new video frame encoded in I420+Alpha format.
        ///     If the internal queue reached its maximum capacity, do nothing and drop the frame.
        /// </summary>
        /// <param name="frame">The video frame to enqueue</param>
        /// <returns>Return <c>true</c> if the frame was enqueued successfully, or <c>false</c> if it was dropped</returns>
        /// <remarks>This should only be used if the queue has storage for a compatible video frame encoding.</remarks>
        public void Enqueue(in I420AVideoFrame frame)
        {
            lock (_lock)
            {
                var container = _currentFrame ?? new I420AVideoFrameContainer();

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

                _currentFrame = container;
            }
        }

        public unsafe void UseCurrentFrame(in FrameRequest request)
        {
            lock (_lock)
            {
                if (_currentFrame == null) return;

                var frame = _currentFrame;
                if (frame.Buffer == null) return;

                fixed (byte* ptr = frame.Buffer)
                {
                    var frameStruct = new I420AVideoFrame
                    {
                        strideA = frame.StrideA,
                        strideV = frame.StrideV,
                        strideY = frame.StrideY,
                        strideU = frame.StrideU,
                        height = frame.Height,
                        width = frame.Width
                    };

                    var dstSizeYA = (ulong) frame.Width * frame.Height;
                    var dstSizeUV = dstSizeYA / 4;

                    void* src = ptr;
                    frameStruct.dataY = new IntPtr(src);

                    src = (void*) ((ulong) src + dstSizeYA);
                    frameStruct.dataU = new IntPtr(src);

                    src = (void*) ((ulong) src + dstSizeUV);
                    frameStruct.dataV = new IntPtr(src);

                    if (frame.StrideA != 0)
                    {
                        src = (void*) ((ulong) src + dstSizeUV);
                        frameStruct.dataA = new IntPtr(src);
                    }
                    else
                    {
                        frameStruct.dataA = IntPtr.Zero;
                    }

                    request.CompleteRequest(in frameStruct);
                }
            }
        }
    }
}