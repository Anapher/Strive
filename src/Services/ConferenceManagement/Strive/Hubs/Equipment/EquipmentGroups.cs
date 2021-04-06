using Strive.Core.Services;

namespace Strive.Hubs.Equipment
{
    public static class EquipmentGroups
    {
        public static string OfParticipant(Participant participant)
        {
            return $"Participant:{participant.ConferenceId}/{participant.Id}";
        }
    }
}
