using Microsoft.MixedReality.WebRTC;

namespace PaderConference.Hubs.Media
{
    public class RTCSessionDescription
    {
        public RTCSessionDescription(SdpMessage sdpMessage)
        {
            Sdp = sdpMessage.Content;
            Type = SdpMessage.TypeToString(sdpMessage.Type);
        }

        public RTCSessionDescription()
        {
        }

        public string? Type { get; set; }

        public string? Sdp { get; set; }

        public SdpMessage ToSdpMessage()
        {
            return new SdpMessage {Type = SdpMessage.StringToType(Type), Content = Sdp};
        }
    }
}