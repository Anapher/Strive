using System.Collections.Generic;
using System.Threading.Tasks;
using Strive.Core.Extensions;
using Strive.Core.Services.Equipment.Gateways;
using Strive.Core.Services.Synchronization;

namespace Strive.Core.Services.Equipment
{
    public class SynchronizedEquipmentProvider : SynchronizedObjectProvider<SynchronizedEquipment>
    {
        private readonly IEquipmentConnectionRepository _repository;

        public SynchronizedEquipmentProvider(IEquipmentConnectionRepository repository)
        {
            _repository = repository;
        }

        public override string Id { get; } = SynchronizedObjectIds.EQUIPMENT;

        public override ValueTask<IEnumerable<SynchronizedObjectId>> GetAvailableObjects(Participant participant)
        {
            return new(SynchronizedEquipment.SyncObjId(participant.Id).Yield());
        }

        protected override async ValueTask<SynchronizedEquipment> InternalFetchValue(string conferenceId,
            SynchronizedObjectId synchronizedObjectId)
        {
            var participantId = synchronizedObjectId.Parameters[SynchronizedEquipment.PROP_PARTICIPANT_ID];
            var joinedParticipant = new Participant(conferenceId, participantId);

            var connections = await _repository.GetConnections(joinedParticipant);
            return new SynchronizedEquipment(connections);
        }
    }
}
