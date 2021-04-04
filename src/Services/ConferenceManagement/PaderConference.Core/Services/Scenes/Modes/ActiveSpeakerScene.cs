namespace PaderConference.Core.Services.Scenes.Modes
{
    public record ActiveSpeakerScene : IScene
    {
        public static readonly ActiveSpeakerScene Instance = new();
    }
}
