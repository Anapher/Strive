namespace Strive.Core.Services.Scenes
{
    public class SceneOptions
    {
        public bool HideParticipantsWithoutWebcam { get; set; } = false;
        public BasicSceneType DefaultScene { get; set; } = BasicSceneType.Grid;

        public enum BasicSceneType
        {
            Grid,
            ActiveSpeaker,
        }
    }
}
