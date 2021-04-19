namespace Strive.Core.Services.Scenes
{
    public interface IScene
    {
    }

    public interface IContentScene : IScene
    {
    }

    public enum SceneType
    {
        Frame,
        Decorator,
        Content,
    }
}
