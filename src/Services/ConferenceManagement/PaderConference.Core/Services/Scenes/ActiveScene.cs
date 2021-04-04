namespace PaderConference.Core.Services.Scenes
{
    public record ActiveScene(bool IsControlled, IScene? Scene, SceneConfig Config);
}
