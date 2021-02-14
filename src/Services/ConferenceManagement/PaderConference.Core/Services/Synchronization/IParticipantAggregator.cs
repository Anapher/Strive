using System.Collections.Generic;
using System.Threading.Tasks;

namespace PaderConference.Core.Services.Synchronization
{
    public interface IParticipantAggregator
    {
        ValueTask<IEnumerable<string>> OfConference(string conferenceId);

        ValueTask<IEnumerable<string>> OfRoom(string conferenceId, string roomId);
    }
}
