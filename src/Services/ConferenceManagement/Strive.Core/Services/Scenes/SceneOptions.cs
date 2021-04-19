using Strive.Core.Services.Scenes.Scenes;

namespace Strive.Core.Services.Scenes
{
    public class SceneOptions
    {
        /// <summary>
        ///     The default state for the default room
        /// </summary>
        public ActiveScene DefaultRoomState { get; set; } = new(true, AutonomousScene.Instance, new SceneConfig(false));

        /// <summary>
        ///     The default state for other rooms
        /// </summary>
        public ActiveScene RoomState { get; set; } = new(false, AutonomousScene.Instance, new SceneConfig(false));
    }
}
