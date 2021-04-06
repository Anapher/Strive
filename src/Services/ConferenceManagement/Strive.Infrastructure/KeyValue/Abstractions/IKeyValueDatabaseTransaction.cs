using System;
using System.Threading.Tasks;

namespace Strive.Infrastructure.KeyValue.Abstractions
{
    public interface IKeyValueDatabaseTransaction : IKeyValueDatabaseActions, IDisposable
    {
        ValueTask<bool> ExecuteAsync();
    }
}
