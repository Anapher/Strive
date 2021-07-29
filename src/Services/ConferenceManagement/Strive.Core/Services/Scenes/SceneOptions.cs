namespace Strive.Core.Services.Scenes
{
    public class SceneOptions
    {
        public BasicSceneType DefaultScene { get; set; } = BasicSceneType.Grid;
        public SceneLayoutType SceneLayout { get; set; } = SceneLayoutType.Auto;
        public SceneLayoutType ScreenShareLayout { get; set; } = SceneLayoutType.ChipsWithPresenter;
        public bool HideParticipantsWithoutWebcam { get; set; } = false;

        public enum BasicSceneType
        {
            Grid,
            ActiveSpeaker,
        }

        public enum SceneLayoutType
        {
            Auto,
            Chips,
            ChipsWithPresenter,
            Tiles,
        }
    }
}
