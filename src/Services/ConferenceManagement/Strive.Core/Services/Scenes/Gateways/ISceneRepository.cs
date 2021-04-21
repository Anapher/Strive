using System.Threading.Tasks;
using Strive.Infrastructure.KeyValue.Abstractions;

namespace Strive.Core.Services.Scenes.Gateways
{
    public interface ISceneRepository
    {
        ValueTask<IAcquiredLock> LockScene(string conferenceId, string roomId);

        ValueTask SetScene(string conferenceId, string roomId, ActiveScene scene);

        ValueTask<ActiveScene?> GetScene(string conferenceId, string roomId);

        ValueTask RemoveScene(string conferenceId, string roomId);

        ValueTask SetSceneState(string conferenceId, string roomId, SceneState state);

        ValueTask<SceneState?> GetSceneState(string conferenceId, string roomId);

        ValueTask RemoveSceneState(string conferenceId, string roomId);
    }
}
