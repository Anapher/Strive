using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using PaderConference.Core.Interfaces;
using PaderConference.Core.Interfaces.Gateways.Repositories;
using PaderConference.Core.Services;
using PaderConference.Core.Services.Media.Communication;
using PaderConference.Core.Services.Media.Mediasoup;
using StackExchange.Redis.Extensions.Core.Abstractions;

namespace PaderConference.Infrastructure.Redis.Repos
{
    public class MediaRepo : IRedisRepo, IMediaRepo
    {
        private readonly IRedisDatabase _database;

        public MediaRepo(IRedisDatabase database)
        {
            _database = database;
        }

        public async Task RegisterConference(string conferenceId)
        {
            await _database.ListAddToLeftAsync(RedisKeys.Media.NewConferencesKey, new ConferenceInfo(conferenceId));
            await _database.PublishAsync<object?>(RedisChannels.Media.NewConferenceCreated, null);
        }

        public async Task<Func<Task>> SubscribeOnSendMessage(string conferenceId,
            Func<SendToConnectionDto, Task> handler)
        {
            var channelName = RedisChannels.OnSendMessageToConnection.GetName(conferenceId);
            await _database.SubscribeAsync(channelName, handler);

            return () => _database.UnsubscribeAsync(channelName, handler);
        }

        public async Task<Func<Task>> SubscribeStreamsChanged(string conferenceId, Func<Task> handler)
        {
            Task HandlerWrapper(object _)
            {
                return handler();
            }

            var channelName = RedisChannels.Media.StreamsChanged.GetName(conferenceId);
            await _database.SubscribeAsync(channelName, (Func<object, Task>) HandlerWrapper);

            return () => _database.UnsubscribeAsync(channelName, (Func<object, Task>) HandlerWrapper);
        }

        public Task<Dictionary<string, ParticipantStreamInfo>> GetStreams(string conferenceId)
        {
            return _database.GetAsync<Dictionary<string, ParticipantStreamInfo>>(
                RedisKeys.Media.Streams.GetName(conferenceId));
        }

        public async Task<JObject?> GetRtpCapabilities(string conferenceId)
        {
            var routerCapabilities = await _database.GetAsync<JObject>(
                RedisKeys.Media.RtpCapabilitiesKey.GetName(conferenceId));
            if (routerCapabilities.Type == JTokenType.Undefined)
                return null;

            return routerCapabilities;
        }

        public Task NotifyClientDisconnected(ConnectionMessageMetadata meta)
        {
            return _database.PublishAsync(RedisChannels.ClientDisconnectedChannel.GetName(meta.ConferenceId),
                new ConnectionMessage<object?>(null, meta));
        }

        public Task<SuccessOrError<JToken?>> SendMessage<TRequest>(ConferenceDependentKey key, string conferenceId,
            ConnectionMessage<TRequest> message)
        {
            return _database.InvokeAsync<JToken?, ConnectionMessage<TRequest>>(key.GetName(conferenceId), message);
        }
    }
}
