using System;
using System.Threading.Tasks;

namespace PaderConference.Infrastructure.Redis.Abstractions
{
    public interface IKeyValueDatabaseTransaction : IKeyValueDatabaseActions, IDisposable
    {
        ValueTask<bool> ExecuteAsync();
    }
}
