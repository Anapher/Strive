using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using PaderConference.Core.Domain.Entities;

namespace PaderConference.Core.Interfaces.Services
{
    public interface IConferenceManager
    {
        event EventHandler<Conference>? ConferenceOpened;
        event EventHandler<string>? ConferenceClosed;

        ValueTask<Conference> OpenConference(string conferenceId);

        ValueTask CloseConference(string conferenceId);

        ValueTask<Participant> Participate(string conferenceId, string participantId, string role, string? displayName);

        ValueTask<bool> GetIsConferenceOpen(string conferenceId);

        ICollection<Participant>? GetParticipants(string conferenceId);

        ValueTask RemoveParticipant(Participant participant);

        string GetConferenceOfParticipant(Participant participant);

        ValueTask SetConferenceState(string conferenceId, ConferenceState state);

        bool TryGetParticipant(string conferenceId, string participantId,
            [NotNullWhen(true)] out Participant? participant);
    }
}