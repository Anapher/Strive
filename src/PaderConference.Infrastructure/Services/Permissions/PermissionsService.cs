using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PaderConference.Core.Domain.Entities;
using PaderConference.Core.Interfaces.Gateways.Repositories;
using PaderConference.Infrastructure.Conferencing;
using PaderConference.Infrastructure.Extensions;
using PaderConference.Infrastructure.Services.Permissions.Dto;
using StackExchange.Redis.Extensions.Core.Abstractions;

namespace PaderConference.Infrastructure.Services.Permissions
{
    public class PermissionsService : ConferenceService, IPermissionsService
    {
        private readonly string _conferenceId;
        private readonly IConferenceRepo _conferenceRepo;
        private readonly DefaultPermissionOptions _defaultPermissions;

        private readonly ConcurrentBag<FetchPermissionsDelegate> _fetchPermissionsDelegates =
            new ConcurrentBag<FetchPermissionsDelegate>();

        private readonly ILogger<PermissionsService> _logger;
        private readonly IRedisDatabase _redisDatabase;

        private IImmutableDictionary<string, JsonElement>? _conferencePermissions;
        private IImmutableDictionary<string, JsonElement>? _moderatorPermissions;
        private IImmutableList<string> _moderators;

        public PermissionsService(string conferenceId, IConferenceRepo conferenceRepo, IRedisDatabase redisDatabase,
            IOptions<DefaultPermissionOptions> defaultPermissions, ILogger<PermissionsService> logger)
        {
            _conferenceId = conferenceId;
            _conferenceRepo = conferenceRepo;
            _redisDatabase = redisDatabase;
            _defaultPermissions = defaultPermissions.Value;
            _logger = logger;

            _moderators = ImmutableList<string>.Empty;

            RegisterLayerProvider(FetchPermissions);
        }

        // TODO: invidivual permissions

        public async ValueTask<IPermissionStack> GetPermissions(Participant participant)
        {
            var layers = new List<PermissionLayer>();
            foreach (var fetchPermissionsDelegate in _fetchPermissionsDelegates)
                layers.AddRange(await fetchPermissionsDelegate(participant));

            return new PermissionStack(layers.OrderBy(x => x.Order).Select(x => x.Permissions).ToList());
        }

        public void RegisterLayerProvider(FetchPermissionsDelegate fetchPermissions)
        {
            _fetchPermissionsDelegates.Add(fetchPermissions);
        }

        private ValueTask<IEnumerable<PermissionLayer>> FetchPermissions(Participant participant)
        {
            var result = new List<PermissionLayer>
            {
                new PermissionLayer(10, _conferencePermissions ?? _defaultPermissions.Conference)
            };

            if (_moderators.Contains(participant.ParticipantId))
                result.Add(new PermissionLayer(30, _moderatorPermissions ?? _defaultPermissions.Moderator));

            return new ValueTask<IEnumerable<PermissionLayer>>(result);
        }

        public override async ValueTask InitializeAsync()
        {
            var conference = await _conferenceRepo.FindById(_conferenceId);
            if (conference == null)
            {
                _logger.LogError("Conference was not found in database.");
                return;
            }

            _conferencePermissions = ParseDictionary(conference.Permissions);
            _moderatorPermissions = ParseDictionary(conference.ModeratorPermissions);
            _moderators = conference.Moderators;

            await _redisDatabase.SubscribeAsync<Conference>(RedisChannels.OnConferenceUpdated(_conferenceId),
                OnConferenceUpdated);
        }

        private static IImmutableDictionary<string, JsonElement>? ParseDictionary(
            IReadOnlyDictionary<string, string>? dictionary)
        {
            return dictionary?.ToImmutableDictionary(x => x.Key,
                x => JsonSerializer.Deserialize<JsonElement>(x.Value));
        }

        public async Task OnRoomPermissionsUpdated(string roomId)
        {
            await _redisDatabase.PublishAsync(RedisChannels.OnPermissionsUpdated,
                new PermissionUpdateDto(_conferenceId, roomId, null));
        }

        public async Task OnParticipantPermissionsUpdated(string participantId)
        {
            await _redisDatabase.PublishAsync(RedisChannels.OnPermissionsUpdated,
                new PermissionUpdateDto(_conferenceId, null, participantId));
        }

        public async Task OnConferencePermissionsUpdated()
        {
            await _redisDatabase.PublishAsync(RedisChannels.OnPermissionsUpdated,
                new PermissionUpdateDto(_conferenceId, null, null));
        }

        public override async ValueTask DisposeAsync()
        {
            await _redisDatabase.UnsubscribeAsync<Conference>(RedisChannels.OnConferenceUpdated(_conferenceId),
                OnConferenceUpdated);
        }

        private async Task OnConferenceUpdated(Conference arg)
        {
            _moderators = arg.Moderators;

            bool PermissionsEqual(IReadOnlyDictionary<string, string>? source,
                IReadOnlyDictionary<string, JsonElement>? target)
            {
                if (source == null && target == null) return true;
                if (source == null || target == null) return false;

                return source.EqualItems(target.ToDictionary(x => x.Key, x => x.Value.ToString()));
            }

            if (!PermissionsEqual(arg.Permissions, _conferencePermissions) ||
                !PermissionsEqual(arg.ModeratorPermissions, _conferencePermissions))
            {
                _conferencePermissions = ParseDictionary(arg.Permissions);
                _moderatorPermissions = ParseDictionary(arg.ModeratorPermissions);

                await OnConferencePermissionsUpdated();
            }
        }
    }
}