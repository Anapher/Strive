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

        public ValueTask<Participant> Participate(string conferenceId, string? displayName)
        {
            if (!Conferences.TryGetValue(conferenceId, out var conference))
                throw new InvalidOperationException($"The conference with id {conferenceId} was not found.");

            var participantId = Guid.NewGuid().ToString("N");
            var participant = new Participant(participantId, displayName, DateTimeOffset.UtcNow);

            _logger.LogDebug("A new user (display name: {name}) want's to participate in {conferenceId}", displayName,
                conferenceId);

            if (!conference.Participants.TryAdd(participantId, participant))
            {
                _logger.LogCritical("A participant id ({id}) was generated that already exists. This must not happen.",
                    participantId);
                return Participate(conferenceId, displayName);
            }

            return new ValueTask<Participant>(participant);
        }
    }
}