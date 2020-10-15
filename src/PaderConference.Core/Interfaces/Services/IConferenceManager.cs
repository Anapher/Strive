using System.Collections.Generic;
using System.Threading.Tasks;
using PaderConference.Core.Domain.Entities;

namespace PaderConference.Core.Interfaces.Services
{
    public interface IConferenceManager
    {
        ValueTask<Conference> StartConference(string conferenceId);

        ValueTask<Participant> Participate(string conferenceId, string userId, string role, string? displayName);

        ValueTask<Conference?> GetConference(string conferenceId);

        ValueTask MarkConferenceAsInactive(string conferenceId);

        ICollection<Participant>? GetParticipants(string conferenceId);

        ValueTask RemoveParticipant(Participant participant);

        string GetConferenceOfParticipant(Participant participant);
    }
}