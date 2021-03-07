using System.Collections.Generic;

namespace PaderConference.Core.Services.Synchronization
{
    public record SynchronizedSubscriptions(IReadOnlyDictionary<string, bool> Subscriptions);
}
