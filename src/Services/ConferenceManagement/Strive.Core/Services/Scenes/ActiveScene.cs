namespace Strive.Core.Services.Scenes
{
    public record ActiveScene(IScene SelectedScene, IScene? OverwrittenContent);
}
