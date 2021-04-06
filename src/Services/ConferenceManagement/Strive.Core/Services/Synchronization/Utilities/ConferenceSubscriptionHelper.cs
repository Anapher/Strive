using System.Collections.Generic;
using System.Linq;

namespace Strive.Core.Services.Synchronization.Utilities
{
    public static class ConferenceSubscriptionHelper
    {
        public static IEnumerable<string> GetParticipantIdsSubscribedTo(
            IReadOnlyDictionary<string, IReadOnlyList<string>?> subscriptions,
            SynchronizedObjectId synchronizedObjectId)
        {
            var idString = synchronizedObjectId.ToString();
            return subscriptions.Where(x => x.Value?.Contains(idString) == true).Select(x => x.Key);
        }
    }
}
