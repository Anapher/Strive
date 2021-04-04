namespace PaderConference.Core.Services.Scenes
{
    public record SceneConfig(bool HideParticipantsWithoutWebcam)
    {
        public static readonly SceneConfig Default = new(false);
    }
}
