using System.Threading;
using System.Threading.Tasks;

namespace Strive.Infrastructure.KeyValue.InMemory
{
    public interface IInMemoryKeyLock
    {
        Task Lock(string key, CancellationToken token);

        void Unlock(string key);
    }
}
