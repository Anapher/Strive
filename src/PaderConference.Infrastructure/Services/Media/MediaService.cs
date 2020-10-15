using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using PaderConference.Core.Domain.Entities;
using PaderConference.Core.Dto;
using PaderConference.Infrastructure.Redis;
using PaderConference.Infrastructure.Services.Media.Communication;
using PaderConference.Infrastructure.Services.Permissions;
using PaderConference.Infrastructure.Services.Synchronization;
using StackExchange.Redis.Extensions.Core.Abstractions;

namespace PaderConference.Infrastructure.Services.Media
{
    public class MediaService : ConferenceService
    {
        private readonly IHubClients _clients;
        private readonly string _conferenceId;
        private readonly ILogger<MediaService> _logger;
        private readonly IPermissionsService _permissionsService;
        private readonly IRedisDatabase _redisDatabase;

        public MediaService(string conferenceId, IHubClients clients, ISynchronizationManager synchronizationManager,
            IRedisDatabase redisDatabase, IPermissionsService permissionsService, ILogger<MediaService> logger)
        {
            _conferenceId = conferenceId;
            _clients = clients;
            _redisDatabase = redisDatabase;
            _permissionsService = permissionsService;
            _logger = logger;
        }

        public override async ValueTask InitializeAsync()
        {
            await _redisDatabase.ListAddToLeftAsync(RedisCommunication.NewConferencesKey,
                new ConferenceInfo(_conferenceId));
            await _redisDatabase.PublishAsync<object?>(RedisCommunication.NewConferenceChannel, null);

            await _redisDatabase.SubscribeAsync<SendToConnectionDto>(
                RedisCommunication.OnSendMessageToConnection.GetName(_conferenceId), OnSendMessageToConnection);
        }

        public override async ValueTask OnClientDisconnected(Participant participant, string connectionId)
        {
            var meta = new ConnectionMessageMetadata(_conferenceId, connectionId, participant.ParticipantId);

            await _redisDatabase.PublishAsync(RedisCommunication.ClientDisconnectedChannel.GetName(_conferenceId),
                new ConnectionMessage<object?>(null, meta));
        }

        public override async ValueTask DisposeAsync()
        {
            await _redisDatabase.UnsubscribeAsync<SendToConnectionDto>(
                RedisCommunication.OnSendMessageToConnection.GetName(_conferenceId), OnSendMessageToConnection);
        }

        private async Task OnSendMessageToConnection(SendToConnectionDto arg)
        {
            await _clients.Client(arg.ConnectionId).SendAsync(arg.MethodName, arg.Payload);
        }

        private ConnectionMessageMetadata GetMeta(IServiceMessage message)
        {
            return new ConnectionMessageMetadata(_conferenceId, message.Context.ConnectionId,
                message.Participant.ParticipantId);
        }

        public async ValueTask<JsonElement> Redirect(IServiceMessage<JsonElement> message,
            RedisCommunication.ChannelName channelName)
        {
            var meta = GetMeta(message);
            var request = new ConnectionMessage<JsonElement>(message.Payload, meta);

            return await _redisDatabase.InvokeAsync<JsonElement, ConnectionMessage<JsonElement>>(
                channelName.GetName(_conferenceId), request);
        }

        public async ValueTask<JsonElement?> GetRouterCapabilities(IServiceMessage message)
        {
            var routerCapabilities = await _redisDatabase.GetAsync<JsonElement>(
                RedisCommunication.RtpCapabilitiesKey.GetName(_conferenceId));
            if (routerCapabilities.ValueKind == JsonValueKind.Undefined)
                return null;

            return routerCapabilities;
        }

        public async ValueTask<Error?> InitializeConnection(IServiceMessage<JsonElement> message)
        {
            var meta = GetMeta(message);
            var request = new ConnectionMessage<JsonElement>(message.Payload, meta);

            try
            {
                await _redisDatabase.InvokeAsync(RedisCommunication.Request.InitializeConnection.GetName(_conferenceId),
                    request);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error occurred on contacting database");
                return new Error("Test", "TODO", -1);
            }

            return null;
        }
    }
}