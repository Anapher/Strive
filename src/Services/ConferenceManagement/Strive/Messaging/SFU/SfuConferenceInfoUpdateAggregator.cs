using System.Collections.Generic;
using System.Collections.Immutable;
using Strive.Messaging.SFU.Dto;

namespace Strive.Messaging.SFU
{
    public class SfuConferenceInfoUpdateAggregator
    {
        private readonly Dictionary<string, string> _participantToRoom = new();
        private readonly Dictionary<string, SfuParticipantPermissions> _permissions = new();
        private readonly HashSet<string> _removedParticipants = new();

        public void Append(SfuConferenceInfoUpdate update)
        {
            foreach (var participant in update.RemovedParticipants)
            {
                _participantToRoom.Remove(participant);
                _removedParticipants.Add(participant);
            }

            foreach (var (participant, roomId) in update.ParticipantToRoom)
            {
                _participantToRoom[participant] = roomId;
                _removedParticipants.Remove(participant);
            }

            foreach (var (participant, permission) in update.ParticipantPermissions)
            {
                _permissions[participant] = permission;
            }
        }

        public SfuConferenceInfoUpdate GetUpdate()
        {
            return new(_participantToRoom.ToImmutableDictionary(), _permissions.ToImmutableDictionary(),
                _removedParticipants.ToImmutableList());
        }
    }
}
