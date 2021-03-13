using System;
using System.Threading.Tasks;

namespace PaderConference.Core.Services.BreakoutRooms.Gateways
{
    public interface IBreakoutRoomsRepository
    {
        ValueTask<BreakoutRoomInternalState?> Get(string conferenceId);

        ValueTask Set(string conferenceId, BreakoutRoomInternalState state);

        ValueTask Remove(string conferenceId);

        ValueTask<IAsyncDisposable> LockBreakoutRooms(string conferenceId);
    }
}
