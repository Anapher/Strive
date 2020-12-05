using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PaderConference.Core.Domain.Entities;
using PaderConference.Core.Interfaces.Gateways.Repositories;
using PaderConference.Core.Interfaces.Services;
using PaderConference.Core.Services;
using StackExchange.Redis.Extensions.Core.Abstractions;

namespace PaderConference.Infrastructure.Conferencing
{
    public class ConferenceManager : IConferenceManager
    {
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

        public event EventHandler<Conference>? ConferenceOpened;
        public event EventHandler<string>? ConferenceClosed;

        public async ValueTask<Conference> OpenConference(string conferenceId)
        {
            using (_logger.BeginScope("StartConference()"))
            using (_logger.BeginScope(new Dictionary<string, object> {{"conferenceId", conferenceId}}))
            {
                var conference = await _conferenceRepo.FindById(conferenceId);
                if (conference == null)
                {
                    _logger.LogDebug("Conference was not found in database");
                    throw new ConferenceNotFoundException(conferenceId);
                }

                if (await _database.HashSetAsync(RedisKeys.OpenConferences, conferenceId, conference))
                {
                    _logger.LogDebug("Conference opened");
                    ConferenceOpened?.Invoke(this, conference);
                }
                else
                {
                    _logger.LogDebug("The conference is already active.");
                }

                return conference;
            }
        }

        public async ValueTask CloseConference(string conferenceId)
        {
            if (await _database.HashDeleteAsync(RedisKeys.OpenConferences, conferenceId))
                ConferenceClosed?.Invoke(this, conferenceId);
        }

        public async ValueTask<bool> GetIsConferenceOpen(string conferenceId)
        {
            return await _database.HashExistsAsync(RedisKeys.OpenConferences, conferenceId);
        }

        public ICollection<Participant> GetParticipants(string conferenceId)
        {
            return _participantsMap.GetParticipants(conferenceId);
        }

        public async ValueTask<Participant> Participate(string conferenceId, string participantId, string role,
            string? displayName)
        {
            using (_logger.BeginScope("Participate()"))
            using (_logger.BeginScope(new Dictionary<string, object>
            {
                {"conferenceId", conferenceId}, {"participantId", participantId},
            }))
            {
                var conference = await _conferenceRepo.FindById(conferenceId);
                if (conference == null)
                {
                    _logger.LogDebug("The conference was not found.");
                    throw new ConferenceNotFoundException(conferenceId);
                }

                // conference must not be open to participate

                _logger.LogDebug("Conference: {@conference}", conference);

                var participant = new Participant(participantId, displayName, role, DateTimeOffset.UtcNow);
                if (!_participantsMap.AddParticipant(conferenceId, participant))
                {
                    _logger.LogError("The participant already participates in the conference.");
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

        public async ValueTask SetConferenceState(string conferenceId, ConferenceState state)
        {
            await _conferenceRepo.SetConferenceState(conferenceId, state);

            var conference = await _conferenceRepo.FindById(conferenceId);
            if (conference == null)
            {
                _logger.LogDebug("The conference was not found.");
                throw new ConferenceNotFoundException(conferenceId);
            }

            await UpdateConference(conference);
        }

        public bool TryGetParticipant(string conferenceId, string participantId,
            [NotNullWhen(true)] out Participant? participant)
        {
            participant = null;
            return _participantsMap.ConferenceParticipants.TryGetValue(conferenceId, out var conferenceParticipants) &&
                   conferenceParticipants.TryGetValue(participantId, out participant);
        }

        private Task UpdateConference(Conference conference)
        {
            return _database.PublishAsync(RedisChannels.OnConferenceUpdated(conference.ConferenceId), conference);
        }
    }
}
