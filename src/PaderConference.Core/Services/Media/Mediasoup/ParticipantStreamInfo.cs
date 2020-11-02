using System.Collections.Generic;

namespace PaderConference.Core.Services.Media.Mediasoup
{
    public class ParticipantStreamInfo
    {
        public Dictionary<string, ConsumerInfo>? Consumers { get; set; }

        public Dictionary<string, ProducerInfo>? Producers { get; set; }
    }

    public class ConsumerInfo
    {
        public bool Paused { get; set; }

        public string? ParticipantId { get; set; }
    }

    public class ProducerInfo
    {
        public bool Paused { get; set; }

        public bool Selected { get; set; }

        public ProducerSource? Kind { get; set; }
    }

    public enum ProducerSource
    {
        Mic,
        Webcam,
        Screen,
    }
}
