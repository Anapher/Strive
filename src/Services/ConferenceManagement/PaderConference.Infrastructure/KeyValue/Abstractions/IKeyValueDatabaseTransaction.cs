using System;
using System.Threading.Tasks;

namespace PaderConference.Infrastructure.KeyValue.Abstractions
{
    public interface IKeyValueDatabaseTransaction : IKeyValueDatabaseActions, IDisposable
    {
        ValueTask<bool> ExecuteAsync();
    }
}
