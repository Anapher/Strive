namespace PaderConference.Hubs.Core
{
    public record SyncObjPayload<T>(string Id, T Value);
}
