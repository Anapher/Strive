using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PaderConference.Core.Domain.Entities;
using PaderConference.Core.Interfaces.Services;

namespace PaderConference.Infrastructure.Conferencing
{
    public class ConferenceManager : IConferenceManager
    {
        private readonly ILogger<ConferenceManager> _logger;

        public ConferenceManager(ILogger<ConferenceManager> logger)
        {
            _logger = logger;
        }

        public ConcurrentDictionary<string, Conference> Conferences { get; } =
            new ConcurrentDictionary<string, Conference>();

        public ValueTask<Conference> CreateConference(string userId, ConferenceSettings? settings)
        {
            var conferenceId = Guid.NewGuid().ToString("D");
            var conference = new Conference(conferenceId, userId, settings);

            _logger.LogDebug("Creating new conference with id {conferenceId} initiated by {userId}", conferenceId,
                userId);

            if (!Conferences.TryAdd(conferenceId, conference))
            {
                _logger.LogCritical("A conference id ({id}) was generated that already exists. This must not happen.",
                    conferenceId);
                return CreateConference(userId, settings);
            }

            return new ValueTask<Conference>(conference);
        }

        public ValueTask RemoveParticipant(Participant participant)
        {
            var conference = participant.Conference;
            conference.Participants.TryRemove(participant.ParticipantId, out _);

            return new ValueTask();
        }

        public ValueTask<Participant> Participate(string conferenceId, string userId, string role, string? displayName)
        {
            if (!Conferences.TryGetValue(conferenceId, out var conference))
                throw new InvalidOperationException($"The conference with id {conferenceId} was not found.");

            var participant = new Participant(userId, displayName, role, DateTimeOffset.UtcNow, conference);

            _logger.LogDebug("A new user (display name: {name}) want's to participate in {conferenceId}", displayName,
                conferenceId);

            if (!conference.Participants.TryAdd(userId, participant))
            {
                _logger.LogCritical("A participant id ({id}) was generated that already exists. This must not happen.",
                    userId);
                return Participate(conferenceId, userId, role, displayName);
            }

            return new ValueTask<Participant>(participant);
        }
    }
}