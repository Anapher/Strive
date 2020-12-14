namespace PaderConference.Core.Services.Scenes
{
    public record RoomSceneState
    {
        public bool IsControlled { get; init; }
        public ConferenceScene? Scene { get; init; }
    }
}
