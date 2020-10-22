using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using PaderConference.Core.Domain.Entities;
using PaderConference.Infrastructure.Redis;
using PaderConference.Infrastructure.Services.Media.Communication;
using PaderConference.Infrastructure.Services.Media.Mediasoup;
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
        private readonly ISynchronizedObject<Dictionary<string, double>> _synchronizedAudioLevels;
        private readonly ISynchronizedObject<Dictionary<string, ParticipantStreamInfo>> _synchronizedStreams;

        public MediaService(string conferenceId, IHubClients clients, ISynchronizationManager synchronizationManager,
            IRedisDatabase redisDatabase, IConnectionMapping connectionMapping, ILogger<MediaService> logger)
        {
            _conferenceId = conferenceId;
            _clients = clients;
            _redisDatabase = redisDatabase;
            _connectionMapping = connectionMapping;
            _logger = logger;

            _synchronizedAudioLevels =
                synchronizationManager.Register("mediaAudioLevel", new Dictionary<string, double>());

            _synchronizedStreams =
                synchronizationManager.Register("mediaStreams", new Dictionary<string, ParticipantStreamInfo>());
        }

        public override async ValueTask InitializeAsync()
        {
            // notify sfu about new conference
            await _redisDatabase.ListAddToLeftAsync(RedisKeys.Media.NewConferencesKey,
                new ConferenceInfo(_conferenceId));
            await _redisDatabase.PublishAsync<object?>(RedisChannels.Media.NewConferenceCreated, null);

            // initialize synchronous message sending
            await _redisDatabase.SubscribeAsync<SendToConnectionDto>(
                RedisChannels.OnSendMessageToConnection.GetName(_conferenceId), OnSendMessageToConnection);

            await _redisDatabase.SubscribeAsync<AudioLevelObserverVolume[]>(
                RedisChannels.Media.AudioObserver.GetName(_conferenceId), OnAudioObserver);

            await _redisDatabase.SubscribeAsync<object?>(RedisChannels.Media.StreamsChanged.GetName(_conferenceId),
                OnStreamsChanged);
        }

        private async Task OnStreamsChanged(object? arg)
        {
            var streams =
                await _redisDatabase.GetAsync<Dictionary<string, ParticipantStreamInfo>>(
                    RedisKeys.Media.Streams.GetName(_conferenceId));

            await _synchronizedStreams.Update(streams);
        }

        private async Task OnAudioObserver(AudioLevelObserverVolume[] arg)
        {
            await _synchronizedAudioLevels.Update(arg.Where(x => x.ParticipantId != null)
                .ToDictionary(x => x.ParticipantId!, x => x.Volume));
        }

        public override async ValueTask OnClientDisconnected(Participant participant)
        {
            if (_connectionMapping.ConnectionsR.TryGetValue(participant, out var connectionId))
            {
                var meta = new ConnectionMessageMetadata(_conferenceId, connectionId, participant.ParticipantId);

                await _redisDatabase.PublishAsync(RedisChannels.ClientDisconnectedChannel.GetName(_conferenceId),
                    new ConnectionMessage<object?>(null, meta));
            }
        }

        public override async ValueTask DisposeAsync()
        {
            await _redisDatabase.UnsubscribeAsync<SendToConnectionDto>(
                RedisChannels.OnSendMessageToConnection.GetName(_conferenceId), OnSendMessageToConnection);

            await _redisDatabase.UnsubscribeAsync<AudioLevelObserverVolume[]>(
                RedisChannels.Media.AudioObserver.GetName(_conferenceId), OnAudioObserver);

            await _redisDatabase.UnsubscribeAsync<object?>(RedisChannels.Media.StreamsChanged.GetName(_conferenceId),
                OnStreamsChanged);
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

        public Func<IServiceMessage<TRequest>, ValueTask<JsonElement?>> Redirect<TRequest>(
            ConferenceDependentKey dependentKey)
        {
            async ValueTask<JsonElement?> Invoke(IServiceMessage<TRequest> message)
            {
                var meta = GetMeta(message);
                var request = new ConnectionMessage<TRequest>(message.Payload, meta);

                return await _redisDatabase.InvokeAsync<JsonElement?, ConnectionMessage<TRequest>>(
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
    }
}