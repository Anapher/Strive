namespace Strive.Core.Services.Scenes
{
    public class SceneOptions
    {
        public bool HideParticipantsWithoutWebcam { get; set; } = false;
        public bool OverlayScene { get; set; } = true;
        public BasicSceneType DefaultScene { get; set; } = BasicSceneType.Grid;

        public enum BasicSceneType
        {
            Grid,
            ActiveSpeaker,
        }
    }
}
