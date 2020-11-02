namespace PaderConference.Core.Services.Equipment.Dto
{
    public class EquipmentAuthResponse
    {
        public EquipmentAuthResponse()
        {
            Success = false;
        }

        public EquipmentAuthResponse(string participantId)
        {
            Success = true;
            ParticipantId = participantId;
        }

        public bool Success { get; }
        public string? ParticipantId { get; }
    }
}
