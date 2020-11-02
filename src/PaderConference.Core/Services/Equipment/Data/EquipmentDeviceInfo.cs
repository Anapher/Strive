using PaderConference.Core.Services.Media.Mediasoup;

namespace PaderConference.Core.Services.Equipment.Data
{
    public class EquipmentDeviceInfo
    {
        public string? DeviceId { get; set; }

        public string? Label { get; set; }

        public ProducerSource Source { get; set; }
    }
}
