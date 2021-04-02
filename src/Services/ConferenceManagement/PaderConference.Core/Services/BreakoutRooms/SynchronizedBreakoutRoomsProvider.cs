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

        public override string Id { get; } = SynchronizedBreakoutRooms.SyncObjId.Id;

        protected override async ValueTask<SynchronizedBreakoutRooms> InternalFetchValue(string conferenceId)
        {
            var current = await _repository.Get(conferenceId);
            return new SynchronizedBreakoutRooms(current?.Config);
        }
    }
}
