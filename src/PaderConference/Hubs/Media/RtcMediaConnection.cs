using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Microsoft.MixedReality.WebRTC;
using PaderConference.Core.Domain.Entities;

namespace PaderConference.Hubs.Media
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

            await _connection.InitializeAsync();
        }

        private void ConnectionOnVideoTrackAdded(RemoteVideoTrack track)
        {
            ScreenShareActivated?.Invoke(this, this);
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
            _connection.CreateAnswer();
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