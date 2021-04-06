namespace Strive.Core.Services.BreakoutRooms.Naming
{
    public interface IRoomNamingStrategy
    {
        string GetName(int index);

        int ParseIndex(string name);
    }
}
