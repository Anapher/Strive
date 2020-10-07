namespace PaderConference.Infrastructure.Services.Synchronization
{
    public interface ISynchronizedObject
    {
        object GetCurrent();
    }
}