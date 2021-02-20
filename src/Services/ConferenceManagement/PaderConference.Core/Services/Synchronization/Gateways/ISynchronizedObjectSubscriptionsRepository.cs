using System.Collections.Generic;
using System.Threading.Tasks;

namespace PaderConference.Core.Services.Synchronization.Gateways
{
    public interface ISynchronizedObjectSubscriptionsRepository
    {
        ValueTask<IReadOnlyList<string>?> GetSet(Participant participant, IReadOnlyList<string> subscriptions);

        ValueTask<IReadOnlyList<string>?> Get(Participant participant);

        ValueTask<IReadOnlyList<string>?> Remove(Participant participant);

        ValueTask<IReadOnlyDictionary<string, IReadOnlyList<string>?>> GetOfConference(string conferenceId);
    }
}
