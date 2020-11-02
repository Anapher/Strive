using System.Threading.Tasks;

namespace PaderConference.Core.Services.Synchronization
{
    public interface ISynchronizedObject<T> : ISynchronizedObject
    {
        T Current { get; }
        ValueTask Update(T newValue);
    }
}