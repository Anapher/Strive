using System.Threading.Tasks;
using PaderConference.Core.Services.BreakoutRooms.Gateways;
using PaderConference.Core.Services.Synchronization;

namespace PaderConference.Core.Services.BreakoutRooms
{
    public class SynchronizedBreakoutRoomsProvider : SynchronizedObjectProviderForAll<SynchronizedBreakoutRooms>
    {
        private readonly IBreakoutRoomRepository _repository;

        public SynchronizedBreakoutRoomsProvider(IBreakoutRoomRepository repository)
        {
            _repository = repository;
        }

        public static SynchronizedObjectId SyncObjId { get; } = new(SynchronizedObjectIds.BREAKOUT_ROOMS);

        public override string Id { get; } = SynchronizedObjectIds.BREAKOUT_ROOMS;

        protected override async ValueTask<SynchronizedBreakoutRooms> InternalFetchValue(string conferenceId)
        {
            var current = await _repository.Get(conferenceId);
            return new SynchronizedBreakoutRooms(current?.Config);
        }
    }
}
