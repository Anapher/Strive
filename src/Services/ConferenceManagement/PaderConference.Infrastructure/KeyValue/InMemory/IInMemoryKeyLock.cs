using System.Threading;
using System.Threading.Tasks;

namespace PaderConference.Infrastructure.KeyValue.InMemory
{
    public interface IInMemoryKeyLock
    {
        Task Lock(string key, CancellationToken token);

        void Unlock(string key);
    }
}
