using System.Collections.Generic;
using System.Threading.Tasks;
using Strive.Infrastructure.KeyValue.Abstractions;

namespace Strive.Core.Services.Scenes.Providers.TalkingStick.Gateways
{
    public interface ITalkingStickRepository
    {
        ValueTask Enqueue(Participant participant, string roomId);

        ValueTask RemoveFromQueue(IEnumerable<Participant> participants, string roomId);

        ValueTask<Participant?> Dequeue(string conferenceId, string roomId);

        ValueTask ClearQueue(string conferenceId, string roomId);

        ValueTask<IReadOnlyList<string>> FetchQueue(string conferenceId, string roomId);

        ValueTask SetCurrentSpeakerAndRemoveFromQueue(Participant participant, string roomId);

        ValueTask RemoveCurrentSpeaker(string conferenceId, string roomId);

        ValueTask<Participant?> GetCurrentSpeaker(string conferenceId, string roomId);

        ValueTask<IAcquiredLock> LockRoom(string conferenceId, string roomId);
    }
}
