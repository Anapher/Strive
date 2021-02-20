using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PaderConference.Core.Services.Synchronization
{
    public interface ISynchronizedObjectProvider
    {
        Type Type { get; }

        string Id { get; }

        ValueTask<object> FetchValue(string conferenceId, SynchronizedObjectId synchronizedObjectId);

        ValueTask<IEnumerable<SynchronizedObjectId>> GetAvailableObjects(Participant participant);
    }
}
