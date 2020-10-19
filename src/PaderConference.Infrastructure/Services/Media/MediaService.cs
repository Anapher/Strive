using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using PaderConference.Core.Domain.Entities;
using PaderConference.Core.Dto;
using PaderConference.Infrastructure.Redis;
using PaderConference.Infrastructure.Services.Media.Communication;
using PaderConference.Infrastructure.Services.Synchronization;
using PaderConference.Infrastructure.Sockets;
using StackExchange.Redis.Extensions.Core.Abstractions;

namespace PaderConference.Infrastructure.Services.Media
{
    public class MediaService : ConferenceService
    {
        private readonly IHubClients _clients;
        private readonly string _conferenceId;
        private readonly ILogger<MediaService> _logger;
        private readonly IConnectionMapping _connectionMapping;
        private readonly IRedisDatabase _redisDatabase;

        public MediaService(string conferenceId, IHubClients clients, ISynchronizationManager synchronizationManager,
            IRedisDatabase redisDatabase, IConnectionMapping connectionMapping, ILogger<MediaService> logger)
        {
            _conferenceId = conferenceId;
            _clients = clients;
            _redisDatabase = redisDatabase;
            _connectionMapping = connectionMapping;
            _logger = logger;
        }

        public override async ValueTask InitializeAsync()
        {
            // notify sfu about new conference
            await _redisDatabase.ListAddToLeftAsync(RedisKeys.Media.NewConferencesKey,
                new ConferenceInfo(_conferenceId));
            await _redisDatabase.PublishAsync<object?>(RedisChannels.Media.NewConferenceCreated, null);

            // initialize synchronous message sending
            await _redisDatabase.SubscribeAsync<SendToConnectionDto>(
                RedisChannels.Media.OnSendMessageToConnection.GetName(_conferenceId), OnSendMessageToConnection);
        }

        public override async ValueTask OnClientDisconnected(Participant participant)
        {
            if (_connectionMapping.ConnectionsR.TryGetValue(participant, out var connectionId))
            {
                var meta = new ConnectionMessageMetadata(_conferenceId, connectionId, participant.ParticipantId);

                await _redisDatabase.PublishAsync(RedisChannels.Media.ClientDisconnectedChannel.GetName(_conferenceId),
                    new ConnectionMessage<object?>(null, meta));
            }
        }

        public override async ValueTask DisposeAsync()
        {
            await _redisDatabase.UnsubscribeAsync<SendToConnectionDto>(
                RedisChannels.Media.OnSendMessageToConnection.GetName(_conferenceId), OnSendMessageToConnection);
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

        public Func<IServiceMessage<JsonElement>, ValueTask<JsonElement?>> Redirect(ConferenceDependentKey dependentKey)
        {
            async ValueTask<JsonElement?> Invoke(IServiceMessage<JsonElement> message)
            {
                var meta = GetMeta(message);
                var request = new ConnectionMessage<JsonElement>(message.Payload, meta);

                return await _redisDatabase.InvokeAsync<JsonElement?, ConnectionMessage<JsonElement>>(
                    dependentKey.GetName(_conferenceId), request);
            }

            return Invoke;
        }

        public async ValueTask<JsonElement?> GetRouterCapabilities(IServiceMessage message)
        {
            var routerCapabilities = await _redisDatabase.GetAsync<JsonElement>(
                RedisKeys.Media.RtpCapabilitiesKey.GetName(_conferenceId));
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
                await _redisDatabase.InvokeAsync(
                    RedisChannels.Media.Request.InitializeConnection.GetName(_conferenceId), request);
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