namespace PaderConference.Core.Services.Scenes
{
    public class GridScene : ConferenceScene
    {
        public override string Type { get; } = "grid";

        public bool HideParticipantsWithoutWebcam { get; set; }
    }
}
