using System.Threading.Tasks;

namespace PaderConference.Infrastructure.Services.Synchronization
{
    public interface ISynchronizedObject<T> : ISynchronizedObject
    {
        T Current { get; }
        ValueTask Update(T newValue);
    }
}