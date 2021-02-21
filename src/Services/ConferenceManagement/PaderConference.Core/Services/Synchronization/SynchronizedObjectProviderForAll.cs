using System.Collections.Generic;
using System.Threading.Tasks;
using PaderConference.Core.Extensions;

namespace PaderConference.Core.Services.Synchronization
{
    public abstract class SynchronizedObjectProviderForAll<T> : SynchronizedObjectProvider<T> where T : class
    {
        public override ValueTask<IEnumerable<SynchronizedObjectId>> GetAvailableObjects(Participant participant)
        {
            return new(new SynchronizedObjectId(Id).Yield());
        }

        protected override ValueTask<T> InternalFetchValue(string conferenceId,
            SynchronizedObjectId synchronizedObjectId)
        {
            return InternalFetchValue(conferenceId);
        }

        protected abstract ValueTask<T> InternalFetchValue(string conferenceId);
    }
}
