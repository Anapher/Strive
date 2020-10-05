using Microsoft.MixedReality.WebRTC;

namespace PaderConference.Hubs.Media
{
    public class RTCIceCandidate
    {
        public RTCIceCandidate()
        {
        }

        public RTCIceCandidate(IceCandidate iceCandidate)
        {
            Candidate = iceCandidate.Content;
            SdpMid = iceCandidate.SdpMid;
            SdpMLineIndex = iceCandidate.SdpMlineIndex;
        }

        public string? Candidate { get; set; }

        public string? SdpMid { get; set; }

        public int? SdpMLineIndex { get; set; }

        public IceCandidate ToIceCandidate()
        {
            return new IceCandidate {Content = Candidate, SdpMid = SdpMid, SdpMlineIndex = SdpMLineIndex.Value};
        }
    }
}