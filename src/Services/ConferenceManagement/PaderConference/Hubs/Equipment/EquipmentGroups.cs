using PaderConference.Core.Services;

namespace PaderConference.Hubs.Equipment
{
    public static class EquipmentGroups
    {
        public static string OfParticipant(Participant participant)
        {
            return $"Participant:{participant.ConferenceId}/{participant.Id}";
        }
    }
}
