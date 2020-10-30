namespace PaderConference.Infrastructure.Services.Equipment.Data
{
    public class UseMediaStateInfo
    {
        public bool Connected { get; set; }

        public bool Enabled { get; set; }

        public bool Paused { get; set; }

        public CurrentStreamInfo? StreamInfo { get; set; }
    }

    public class CurrentStreamInfo
    {
        public string? ProducerId { get; set; }

        public string? DeviceId { get; set; }
    }
}
