namespace PaderConference.Core.Services.Scenes.Modes
{
    public class ScreenShareScene : ConferenceScene
    {
        public ScreenShareScene(string participantId)
        {
            ParticipantId = participantId;
        }

        public override string Type { get; } = "screenshare";

        public string ParticipantId { get; set; }
    }
}
