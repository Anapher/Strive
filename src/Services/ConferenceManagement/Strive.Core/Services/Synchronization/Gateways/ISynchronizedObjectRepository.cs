using System;
using System.Threading.Tasks;

namespace Strive.Core.Services.Synchronization.Gateways
{
    public interface ISynchronizedObjectRepository
    {
        ValueTask<object?> Create(string conferenceId, string syncObjId, object newValue, Type type);

        ValueTask<object?> Get(string conferenceId, string syncObjId, Type type);

        ValueTask Remove(string conferenceId, string syncObjId);
    }
}
