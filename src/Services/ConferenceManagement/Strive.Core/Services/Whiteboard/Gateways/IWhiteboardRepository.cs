using System.Collections.Generic;
using System.Threading.Tasks;

namespace Strive.Core.Services.Whiteboard.Gateways
{
    public interface IWhiteboardRepository
    {
        ValueTask Create(string conferenceId, string roomId, Whiteboard whiteboard);

        ValueTask<IReadOnlyList<Whiteboard>> GetAll(string conferenceId, string roomId);

        ValueTask<Whiteboard?> Get(string conferenceId, string roomId, string whiteboardId);

        ValueTask Delete(string conferenceId, string roomId, string whiteboardId);

        ValueTask DeleteAllOfRoom(string conferenceId, string roomId);
    }
}
