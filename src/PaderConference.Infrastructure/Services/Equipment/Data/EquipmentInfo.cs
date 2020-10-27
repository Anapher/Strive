namespace PaderConference.Infrastructure.Services.Equipment.Data
{
    public class EquipmentInfo
    {
        public EquipmentInfo(string connectionId)
        {
            ConnectionId = connectionId;
        }

        public string ConnectionId { get; }
    }
}
