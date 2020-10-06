using System;
using Microsoft.MixedReality.WebRTC;

namespace PaderConference.Hubs.Media
{
    public class VideoTrackRedirect : IDisposable
    {
        private readonly VideoFrameQueue<I420AVideoFrameStorage> _queue = new VideoFrameQueue<I420AVideoFrameStorage>(1);


        public VideoTrackRedirect(RemoteVideoTrack track)
        {
            track.I420AVideoFrameReady += TrackOnI420AVideoFrameReady;
            Source = ExternalVideoTrackSource.CreateFromI420ACallback(FrameCallback);
        }

        public VideoTrackSource Source { get; }

        public void Dispose()
        {
            Source.Dispose();
        }

        private void TrackOnI420AVideoFrameReady(I420AVideoFrame frame)
        {
            _queue.Enqueue(frame);
        }

        private void FrameCallback(in FrameRequest request)
        {
            if (_queue.TryDequeue(out var frame))
            {
                var frame = new 
                request.CompleteRequest(frame.);
            }
        }
    }

    public class CachedFrame : IDisposable
    {
        private readonly byte[] _buffer;

        public static CachedFrame FromI420AVideoFrame(I420AVideoFrame frame)
        {
            uint pixelSize = frame.width * frame.height;
            uint byteSize = (pixelSize / 2 * 3); // I420 = 12 bits per pixel

            var buffer = new byte[byteSize];
            frame.CopyTo(buffer);


            var bufferSize = frame.CopyTo()
        }

        public void Dispose()
        {
        }
    }
}