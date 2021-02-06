using System;
using System.Threading.Tasks;

namespace PaderConference.Core.Services.Synchronization.Gateways
{
    public interface ISynchronizedObjectRepository
    {
        Task<T?> Update<T>(string conferenceId, string name, T value);

        Task<(T? previousValue, T newValue)> Update<T>(string conferenceId, string name, Func<T?, T> updateValueFn);
    }
}
