namespace PaderConference.Core.Services.BreakoutRoom.Naming
{
    public interface IRoomNamingStrategy
    {
        string GetName(int index);

        int ParseIndex(string name);
    }
}
