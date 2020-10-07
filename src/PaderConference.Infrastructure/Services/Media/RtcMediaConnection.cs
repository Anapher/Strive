using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Microsoft.MixedReality.WebRTC;
using PaderConference.Core.Domain.Entities;
using PaderConference.Infrastructure.Services.Media.Data;
using PaderConference.Infrastructure.Services.Media.Utils;

namespace PaderConference.Infrastructure.Services.Media
{
    public class RtcMediaConnection : IDisposable
    {
        private readonly PeerConnection _connection;
        private readonly ILogger<RtcMediaConnection> _logger;
        private readonly IClientProxy _signal;

        public RtcMediaConnection(Participant participant, IClientProxy signal, ILogger<RtcMediaConnection> logger)
        {
            _signal = signal;
            _logger = logger;
            Participant = participant;
            _connection = new PeerConnection();
        }

        public VideoTrackRedirect? VideoTrack { get; private set; }

        public Participant Participant { get; }

        public void Dispose()
        {
            _connection.Dispose();
        }

        public event EventHandler<RtcMediaConnection>? ScreenShareActivated;

        public async ValueTask Init()
        {
            _connection.IceCandidateReadytoSend += ConnectionOnIceCandidateReadytoSend;
            _connection.LocalSdpReadytoSend += ConnectionOnLocalSdpReadytoSend;
            _connection.Connected += ConnectionOnConnected;
            _connection.VideoTrackAdded += ConnectionOnVideoTrackAdded;
            _connection.RenegotiationNeeded += ConnectionOnRenegotiationNeeded;

            await _connection.InitializeAsync(new PeerConnectionConfiguration
                {IceTransportType = IceTransportType.All});
        }

        private void ConnectionOnRenegotiationNeeded()
        {
        }

        private void ConnectionOnVideoTrackAdded(RemoteVideoTrack track)
        {
            VideoTrack = new VideoTrackRedirect(track);
            ScreenShareActivated?.Invoke(this, this);

            track.Argb32VideoFrameReady += TrackOnArgb32VideoFrameReady;
            track.I420AVideoFrameReady += TrackOnI420AVideoFrameReady;
        }

        private void TrackOnI420AVideoFrameReady(I420AVideoFrame frame)
        {
        }

        private void TrackOnArgb32VideoFrameReady(Argb32VideoFrame frame)
        {
        }

        private void ConnectionOnConnected()
        {
            _logger.LogDebug("Connection initialized");
        }

        public void OnIceCandidate(RTCIceCandidate iceCandidate)
        {
            _connection.AddIceCandidate(iceCandidate.ToIceCandidate());
        }

        public async Task InitializeInfo(RTCSessionDescription sessionDescription)
        {
            await _connection.SetRemoteDescriptionAsync(sessionDescription.ToSdpMessage());

            if (sessionDescription.Type == "offer")
                if (!_connection.CreateAnswer())
                    throw new InvalidOperationException("Creating answer failed");
        }

        public void AddVideo(VideoTrackRedirect videoTrackRedirect)
        {
            var tranceiver = _connection.AddTransceiver(MediaKind.Video,
                new TransceiverInitSettings
                    {InitialDesiredDirection = Transceiver.Direction.SendOnly, Name = "Screenshare"});

            tranceiver.LocalVideoTrack =
                LocalVideoTrack.CreateFromSource(videoTrackRedirect.CreateSource(), new LocalVideoTrackInitConfig());

            _connection.CreateOffer();
        }

        private void ConnectionOnIceCandidateReadytoSend(IceCandidate candidate)
        {
            _signal.SendAsync(CoreHubMessages.Response.OnIceCandidate, new RTCIceCandidate(candidate));
        }

        private void ConnectionOnLocalSdpReadytoSend(SdpMessage message)
        {
            _signal.SendAsync(CoreHubMessages.Response.OnSdp, new RTCSessionDescription(message));
        }
    }
}