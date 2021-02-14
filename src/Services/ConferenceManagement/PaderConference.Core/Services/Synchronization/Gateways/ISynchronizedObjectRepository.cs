using System.Threading.Tasks;

namespace PaderConference.Core.Services.Synchronization.Gateways
{
    public interface ISynchronizedObjectRepository
    {
        ValueTask<object?> Create(string conferenceId, string syncObjId, object newValue);

        ValueTask Remove(string conferenceId, string syncObjId);
    }
}
