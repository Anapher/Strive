using Microsoft.MixedReality.WebRTC;
using PaderConference.Infrastructure.WebRtc;

namespace PaderConference.Hubs.Media
{
    public class VideoTrackRedirect
    {
        private readonly AsyncVideoFrameQueue _queue = new AsyncVideoFrameQueue();

        public VideoTrackRedirect(RemoteVideoTrack track)
        {
            track.I420AVideoFrameReady += TrackOnI420AVideoFrameReady;
            Track = track;
        }

        public RemoteVideoTrack Track { get; }

        public VideoTrackSource CreateSource()
        {
            return ExternalVideoTrackSource.CreateFromI420ACallback(FrameCallback);
        }

        private void TrackOnI420AVideoFrameReady(I420AVideoFrame frame)
        {
            _queue.Enqueue(frame);
        }

        private void FrameCallback(in FrameRequest request)
        {
            _queue.UseCurrentFrame(in request);
        }
    }
}