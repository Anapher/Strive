using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using PaderConference.Core.Domain.Entities;
using PaderConference.Infrastructure.Services.Media.Communication;
using PaderConference.Infrastructure.Services.Permissions;
using PaderConference.Infrastructure.Services.Synchronization;
using StackExchange.Redis.Extensions.Core.Abstractions;

namespace PaderConference.Infrastructure.Services.Media
{
    public class MediaService : ConferenceService
    {
        private readonly IHubClients _clients;
        private readonly Conference _conference;
        private readonly IPermissionsService _permissionsService;
        private readonly IRedisDatabase _redisDatabase;

        public MediaService(Conference conference, IHubClients clients, ISynchronizationManager synchronizationManager,
            IRedisDatabase redisDatabase, IPermissionsService permissionsService)
        {
            _conference = conference;
            _clients = clients;
            _redisDatabase = redisDatabase;
            _permissionsService = permissionsService;
        }

        public override async ValueTask InitializeAsync()
        {
            await _redisDatabase.ListAddToLeftAsync(RedisCommunication.NewConferencesKey,
                new ConferenceInfo(_conference));
            await _redisDatabase.PublishAsync<object?>(RedisCommunication.NewConferenceChannel, null);
        }

        public async ValueTask InitializeConnection(IServiceMessage<JsonElement> message)
        {
            var meta = new ConnectionMessageMetadata(_conference.ConferenceId, message.Context.ConnectionId,
                message.Participant.ParticipantId);
            var request = new ConnectionMessage<JsonElement>(message.Payload, meta);

            await _redisDatabase.PublishAsync(
                RedisCommunication.Request.InitializeConnection.GetName(_conference.ConferenceId),
                request);

            var info = await _redisDatabase.GetAsync<JsonElement>(
                RedisCommunication.RtpCapabilitiesKey.GetName(_conference.ConferenceId));
            await message.Clients.Caller.SendAsync(CoreHubMessages.Response.OnRouterCapabilities, info);
        }

        public async ValueTask CreateTransport(IServiceMessage<JsonElement> message)
        {
            // SECURITY: if participant can produce audio/video
            if (!(await _permissionsService.GetPermissions(message.Participant)).CanShareMedia()) return;

            var meta = new ConnectionMessageMetadata(_conference.ConferenceId, message.Context.ConnectionId,
                message.Participant.ParticipantId);

            var request = new ConnectionMessage<JsonElement>(message.Payload, meta);

            await _redisDatabase.PublishAsync(
                RedisCommunication.Request.CreateTransport.GetName(_conference.ConferenceId),
                request);
        }
    }
}