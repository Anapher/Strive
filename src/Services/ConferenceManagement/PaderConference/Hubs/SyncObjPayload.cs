namespace PaderConference.Hubs
{
    public record SyncObjPayload<T>(string Id, T Value);
}
