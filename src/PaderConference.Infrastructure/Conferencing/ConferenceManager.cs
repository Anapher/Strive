using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PaderConference.Core.Domain.Entities;
using PaderConference.Core.Interfaces.Services;
using StackExchange.Redis.Extensions.Core.Abstractions;

namespace PaderConference.Infrastructure.Conferencing
{
    public class ConferenceManager : IConferenceManager
    {
        private const string RedisConferencesKey = "conferences";
        private readonly IRedisDatabase _database;
        private readonly ILogger<ConferenceManager> _logger;

        public ConferenceManager(IRedisDatabase database, ILogger<ConferenceManager> logger)
        {
            _database = database;
            _logger = logger;
        }

        public ConcurrentDictionary<string, string> ParticipantToConference { get; } =
            new ConcurrentDictionary<string, string>();

        public ConcurrentDictionary<string, ConcurrentDictionary<string, Participant>> ConferenceParticipants { get; } =
            new ConcurrentDictionary<string, ConcurrentDictionary<string, Participant>>();

        public async ValueTask<Conference> StartConference(string conferenceId)
        {
            throw new NotImplementedException();
        }

        public async ValueTask<Conference?> GetConference(string conferenceId)
        {
            return await _database.HashGetAsync<Conference>(RedisConferencesKey, conferenceId);
        }

        public ValueTask MarkConferenceAsInactive(string conferenceId)
        {
            throw new NotImplementedException();
        }

        public ICollection<Participant>? GetParticipants(string conferenceId)
        {
            if (ConferenceParticipants.TryGetValue(conferenceId, out var participants))
                return participants.Values;

            return ImmutableList<Participant>.Empty;
        }

        public async ValueTask<Participant> Participate(string conferenceId, string userId, string role,
            string? displayName)
        {
            var conference = await GetConference(conferenceId);
            if (conference == null)
                throw new InvalidOperationException($"The conference with id {conferenceId} was not found.");

            var participant = new Participant(userId, displayName, role, DateTimeOffset.UtcNow);

            _logger.LogDebug("A new user (display name: {name}) want's to participate in {conferenceId}", displayName,
                conferenceId);

            var participants =
                ConferenceParticipants.GetOrAdd(conferenceId, _ => new ConcurrentDictionary<string, Participant>());

            if (!participants.TryAdd(userId, participant))
            {
                _logger.LogCritical("A participant id ({id}) was generated that already exists. This must not happen.",
                    userId);
                return await Participate(conferenceId, userId, role, displayName);
            }

            ParticipantToConference.TryAdd(participant.ParticipantId, conferenceId);

            return participant;
        }

        public ValueTask RemoveParticipant(Participant participant)
        {
            if (!ParticipantToConference.TryRemove(participant.ParticipantId, out var conferenceId))
                throw new InvalidOperationException("The participant wasn't in a conference.");

            if (ConferenceParticipants.TryGetValue(conferenceId, out var participants))
                participants.TryRemove(participant.ParticipantId, out _);

            return new ValueTask();
        }

        public string GetConferenceOfParticipant(Participant participant)
        {
            return ParticipantToConference[participant.ParticipantId];
        }

        //public async ValueTask<Conference> CreateConference(string userId, ConferenceSettings? settings)
        //{
        //    var conferenceId = Guid.NewGuid().ToString("D");
        //    var conference = new Conference(conferenceId, userId, settings);

        //    _logger.LogDebug("Creating new conference with id {conferenceId} initiated by {userId}", conferenceId,
        //        userId);

        //    if (!await _database.HashSetAsync(RedisConferencesKey, conferenceId, conference))
        //    {
        //        _logger.LogCritical("A conference id ({id}) was generated that already exists. This must not happen.",
        //            conferenceId);
        //        return await CreateConference(userId, settings);
        //    }

        //    return conference;
        //}
    }
}