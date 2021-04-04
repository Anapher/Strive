using System.Collections.Generic;
using System.Threading.Tasks;

namespace PaderConference.Core.Services.Scenes.Gateways
{
    public interface ISceneRepository
    {
        ValueTask SetScene(string conferenceId, string roomId, ActiveScene scene);

        ValueTask<ActiveScene?> GetScene(string conferenceId, string roomId);

        ValueTask RemoveScene(string conferenceId, string roomId);

        ValueTask SetAvailableScenes(string conferenceId, string roomId, IReadOnlyList<IScene> scenes);

        ValueTask<IReadOnlyList<IScene>?> GetAvailableScenes(string conferenceId, string roomId);

        ValueTask RemoveAvailableScenes(string conferenceId, string roomId);
    }
}
