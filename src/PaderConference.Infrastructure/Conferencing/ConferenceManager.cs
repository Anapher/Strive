using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PaderConference.Core.Domain.Entities;
using PaderConference.Core.Interfaces.Gateways.Repositories;
using PaderConference.Core.Interfaces.Services;
using StackExchange.Redis.Extensions.Core.Abstractions;

namespace PaderConference.Infrastructure.Conferencing
{
    public class ConferenceManager : IConferenceManager
    {
        private const string RedisActiveConferencesKey = "conferences";
        private readonly IConferenceRepo _conferenceRepo;
        private readonly IRedisDatabase _database;
        private readonly ILogger<ConferenceManager> _logger;
        private readonly ParticipantsMap _participantsMap = new ParticipantsMap();

        public ConferenceManager(IRedisDatabase database, IConferenceRepo conferenceRepo,
            ILogger<ConferenceManager> logger)
        {
            _database = database;
            _conferenceRepo = conferenceRepo;
            _logger = logger;
        }

        public async ValueTask<Conference> StartConference(string conferenceId)
        {
            using (_logger.BeginScope("StartConference()"))
            using (_logger.BeginScope(new Dictionary<string, object> {{"conferenceId", conferenceId}}))
            {
                var conference = await _conferenceRepo.FindById(conferenceId);
                if (conference == null)
                {
                    _logger.LogDebug("Conference was not found in database");
                    throw new InvalidOperationException($"The conference {conferenceId} was not found in database.");
                }

                if (!await _database.HashSetAsync(RedisActiveConferencesKey, conferenceId, conference))
                    _logger.LogDebug("The conference is already active.");
                else
                    _logger.LogDebug("Conference activated");

                return conference;
            }
        }

        public async ValueTask CloseConference(string conferenceId)
        {
            if (await _database.HashDeleteAsync(RedisActiveConferencesKey, conferenceId))
                _participantsMap.RemoveConference(conferenceId);
        }

        public async ValueTask MarkConferenceAsInactive(string conferenceId)
        {
            await _conferenceRepo.SetConferenceState(conferenceId, ConferenceState.Inactive);
        }

        public async ValueTask<bool> GetIsConferenceStarted(string conferenceId)
        {
            return await _database.HashExistsAsync(RedisActiveConferencesKey, conferenceId);
        }

        public ICollection<Participant>? GetParticipants(string conferenceId)
        {
            return _participantsMap.GetParticipants(conferenceId);
        }

        public async ValueTask<Participant> Participate(string conferenceId, string userId, string role,
            string? displayName)
        {
            using (_logger.BeginScope("Participate()"))
            using (_logger.BeginScope(new Dictionary<string, object>
                {{"conferenceId", conferenceId}, {"userId", userId}}))
            {
                if (!await GetIsConferenceStarted(conferenceId))
                {
                    _logger.LogDebug("The conference is not active.");
                    throw new InvalidOperationException(
                        $"The conference with id {conferenceId} is not currently active.");
                }

                var participant = new Participant(userId, displayName, role, DateTimeOffset.UtcNow);
                if (!_participantsMap.AddParticipant(conferenceId, participant))
                {
                    _logger.LogDebug("The participant already participates in the conference.");
                    throw new InvalidOperationException("The participant already participates in the conference.");
                }

                return participant;
            }
        }

        public ValueTask RemoveParticipant(Participant participant)
        {
            _participantsMap.RemoveParticipant(participant.ParticipantId);
            return new ValueTask();
        }

        public string GetConferenceOfParticipant(Participant participant)
        {
            return _participantsMap.ParticipantToConference[participant.ParticipantId];
        }
    }
}