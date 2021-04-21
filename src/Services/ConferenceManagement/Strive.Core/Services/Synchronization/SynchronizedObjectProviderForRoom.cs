using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Strive.Core.Extensions;
using Strive.Core.Services.Rooms;
using Strive.Core.Services.Synchronization.Extensions;

namespace Strive.Core.Services.Synchronization
{
    public abstract class SynchronizedObjectProviderForRoom<T> : SynchronizedObjectProvider<T> where T : class
    {
        public const string PROP_ROOMID = "roomId";

        private readonly IMediator _mediator;

        protected SynchronizedObjectProviderForRoom(IMediator mediator)
        {
            _mediator = mediator;
        }

        public override async ValueTask<IEnumerable<SynchronizedObjectId>> GetAvailableObjects(Participant participant)
        {
            var rooms = await _mediator.FetchSynchronizedObject<SynchronizedRooms>(participant.ConferenceId,
                SynchronizedRooms.SyncObjId);

            if (!rooms.Participants.TryGetValue(participant.Id, out var roomId))
                return Enumerable.Empty<SynchronizedObjectId>();

            return BuildSyncObjId(Id, roomId).Yield();
        }

        protected override ValueTask<T> InternalFetchValue(string conferenceId,
            SynchronizedObjectId synchronizedObjectId)
        {
            var roomId = synchronizedObjectId.Parameters[PROP_ROOMID];
            return InternalFetchValue(conferenceId, roomId);
        }

        protected abstract ValueTask<T> InternalFetchValue(string conferenceId, string roomId);

        public static SynchronizedObjectId BuildSyncObjId(string baseId, string roomId)
        {
            return new(baseId, new Dictionary<string, string> {{PROP_ROOMID, roomId}});
        }
    }
}
