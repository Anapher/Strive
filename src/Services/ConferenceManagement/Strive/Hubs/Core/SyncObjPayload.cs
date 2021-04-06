namespace Strive.Hubs.Core
{
    public record SyncObjPayload<T>(string Id, T Value);
}
