namespace PaderConference.Infrastructure.Services.Synchronization
{
    public interface ISynchronizationManager
    {
        ISynchronizedObject<T> Register<T>(string name, T initialValue) where T : notnull;
    }
}