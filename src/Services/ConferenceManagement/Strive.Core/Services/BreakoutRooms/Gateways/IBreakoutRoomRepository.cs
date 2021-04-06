using System;
using System.Threading.Tasks;
using Strive.Core.Interfaces.Gateways;

namespace Strive.Core.Services.BreakoutRooms.Gateways
{
    public interface IBreakoutRoomRepository : IStateRepository
    {
        ValueTask<BreakoutRoomInternalState?> Get(string conferenceId);

        ValueTask Set(string conferenceId, BreakoutRoomInternalState state);

        ValueTask Remove(string conferenceId);

        ValueTask<IAsyncDisposable> LockBreakoutRooms(string conferenceId);
    }
}
