using System;
using System.Threading.Tasks;
using PaderConference.Core.Interfaces.Gateways;

namespace PaderConference.Core.Services.BreakoutRooms.Gateways
{
    public interface IBreakoutRoomRepository : IStateRepository
    {
        ValueTask<BreakoutRoomInternalState?> Get(string conferenceId);

        ValueTask Set(string conferenceId, BreakoutRoomInternalState state);

        ValueTask Remove(string conferenceId);

        ValueTask<IAsyncDisposable> LockBreakoutRooms(string conferenceId);
    }
}
