using PaderConference.Core.Services.Scenes.Modes;

namespace PaderConference.Core.Services.Scenes
{
    public class ScenesOptions
    {
        /// <summary>
        ///     The default state for the default room
        /// </summary>
        public RoomSceneState DefaultRoomState { get; set; } =
            new() {IsControlled = true, Scene = AutomaticScene.Instance};

        /// <summary>
        ///     The default state for other rooms
        /// </summary>
        public RoomSceneState RoomState { get; set; } = new() {IsControlled = false, Scene = AutomaticScene.Instance};
    }
}
