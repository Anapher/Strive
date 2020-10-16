using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using PaderConference.Core.Domain.Entities;

namespace PaderConference.Infrastructure.Conferencing
{
    public class ParticipantsMap
    {
        public ConcurrentDictionary<string, string> ParticipantToConference { get; } =
            new ConcurrentDictionary<string, string>();

        public ConcurrentDictionary<string, ConcurrentDictionary<string, Participant>> ConferenceParticipants { get; } =
            new ConcurrentDictionary<string, ConcurrentDictionary<string, Participant>>();

        public ICollection<Participant>? GetParticipants(string conferenceId)
        {
            if (ConferenceParticipants.TryGetValue(conferenceId, out var participants))
                return participants.Values;

            return ImmutableList<Participant>.Empty;
        }

        public bool AddParticipant(string conferenceId, Participant participant)
        {
            var participants =
                ConferenceParticipants.GetOrAdd(conferenceId, _ => new ConcurrentDictionary<string, Participant>());

            if (!participants.TryAdd(participant.ParticipantId, participant)) return false;
            ParticipantToConference[participant.ParticipantId] = conferenceId;

            return true;
        }

        public void RemoveParticipant(string participantId)
        {
            if (!ParticipantToConference.TryRemove(participantId, out var conferenceId))
                return;

            if (ConferenceParticipants.TryGetValue(conferenceId, out var participants))
                participants.TryRemove(participantId, out _);
        }

        public void RemoveConference(string conferenceId)
        {
            if (ConferenceParticipants.TryRemove(conferenceId, out var participants))
                foreach (var participantId in participants.Keys)
                    ParticipantToConference.TryRemove(participantId, out _);
        }
    }
}