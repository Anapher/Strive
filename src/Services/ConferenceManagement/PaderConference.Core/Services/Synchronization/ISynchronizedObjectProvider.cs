using System;
using System.Threading.Tasks;

namespace PaderConference.Core.Services.Synchronization
{
    public interface ISynchronizedObjectProvider
    {
        Type SynchronizedObjectType { get; }

        ValueTask<bool> CanSubscribe(string conferenceId, string participantId);

        ValueTask<object> FetchValue(string conferenceId, string participantId);

        ValueTask<string> GetSynchronizedObjectId(string conferenceId, string participantId);
    }
}
