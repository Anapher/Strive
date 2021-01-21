namespace PaderConference.Core.Services.Scenes.Modes
{
    public class AutomaticScene : ConferenceScene
    {
        public static AutomaticScene Instance = new AutomaticScene();

        public override string Type { get; } = "automatic";
    }
}
