using System.Threading.Tasks;
using Strive.Core.Services.BreakoutRooms.Gateways;
using Strive.Core.Services.Synchronization;

namespace Strive.Core.Services.BreakoutRooms
{
    public class SynchronizedBreakoutRoomsProvider : SynchronizedObjectProviderForAll<SynchronizedBreakoutRooms>
    {
        private readonly IBreakoutRoomRepository _repository;

        public SynchronizedBreakoutRoomsProvider(IBreakoutRoomRepository repository)
        {
            _repository = repository;
        }

        public override string Id { get; } = SynchronizedBreakoutRooms.SyncObjId.Id;

        protected override async ValueTask<SynchronizedBreakoutRooms> InternalFetchValue(string conferenceId)
        {
            var current = await _repository.Get(conferenceId);
            return new SynchronizedBreakoutRooms(current?.Config);
        }
    }
}
