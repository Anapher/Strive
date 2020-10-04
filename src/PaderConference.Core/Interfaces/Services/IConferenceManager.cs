using System.Collections.Concurrent;
using System.Threading.Tasks;
using PaderConference.Core.Domain.Entities;

namespace PaderConference.Core.Interfaces.Services
{
    public interface IConferenceManager
    {
        ConcurrentDictionary<string, Conference> Conferences { get; }

        ValueTask<Conference> CreateConference(string userId, ConferenceSettings? settings);

        ValueTask<Participant> Participate(string conferenceId, string? displayName);

        ValueTask RemoveParticipant(Participant participant);
    }
}