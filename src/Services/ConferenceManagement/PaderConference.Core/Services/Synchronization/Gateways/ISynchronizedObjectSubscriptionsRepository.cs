using System.Collections.Generic;
using System.Threading.Tasks;

namespace PaderConference.Core.Services.Synchronization.Gateways
{
    public interface ISynchronizedObjectSubscriptionsRepository
    {
        ValueTask<IReadOnlyList<string>?> GetSet(string conferenceId, string participantId,
            IReadOnlyList<string> subscriptions);

        ValueTask<IReadOnlyList<string>?> Get(string conferenceId, string participantId);

        ValueTask<IReadOnlyList<string>?> Remove(string conferenceId, string participantId);

        ValueTask<IReadOnlyDictionary<string, IReadOnlyList<string>?>> GetOfConference(string conferenceId);
    }
}
