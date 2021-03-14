using System;

namespace PaderConference.Infrastructure.KeyValue
{
    public class KeyValueDatabaseOptions
    {
        /// <summary>
        ///     Specifies how long the lock will last, absent auto-extension. Because auto-extension exists,
        ///     this value generally will have little effect on program behavior. However, making the expiry longer means that
        ///     auto-extension requests can occur less frequently, saving resources. On the other hand, when a lock is abandoned
        ///     without explicit release (e. g. if the holding process crashes), the expiry determines how long other processes
        ///     would need to wait in order to acquire it.
        ///     Defaults to 30s.
        /// </summary>
        public TimeSpan LockExpiry { get; set; } = TimeSpan.FromSeconds(30);
    }
}
