using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using PaderConference.Core.Domain.Entities;
using PaderConference.Infrastructure.Services.Media.Data;
using PaderConference.Infrastructure.Services.Synchronization;

namespace PaderConference.Infrastructure.Services.Media
{
    public class MediaService : ConferenceService
    {
        private readonly IHubClients _clients;
        private readonly Conference _conference;
        private readonly IRtcMediaConnectionFactory _connectionFactory;

        private readonly ConcurrentDictionary<Participant, RtcMediaConnection> _connections =
            new ConcurrentDictionary<Participant, RtcMediaConnection>();

        private readonly ISynchronizedObject<SynchronizedMedia> _synchronizedMedia;

        public MediaService(Conference conference, IHubClients clients,
            IRtcMediaConnectionFactory connectionFactory, ISynchronizationManager synchronizationManager)
        {
            _conference = conference;
            _clients = clients;
            _connectionFactory = connectionFactory;

            _synchronizedMedia = synchronizationManager.Register("media", new SynchronizedMedia());
        }

        public RtcMediaConnection? CurrentScreenShare { get; private set; }

        public override ValueTask DisposeAsync()
        {
            while (!_connections.IsEmpty)
            {
                var item = _connections.Take(1).ToList();
                if (!item.Any()) break;

                if (_connections.TryRemove(item[0].Key, out var connection))
                    connection.Dispose();
            }

            return new ValueTask();
        }

        public override ValueTask OnClientDisconnected(Participant participant)
        {
            if (CurrentScreenShare?.Participant.ParticipantId == participant.ParticipantId)
            {
                CurrentScreenShare = null;
                _synchronizedMedia.Update(new SynchronizedMedia());
            }

            if (_connections.TryRemove(participant, out var connection)) connection.Dispose();

            return new ValueTask();
        }

        public async ValueTask OnIceCandidate(IServiceMessage<RTCIceCandidate> message)
        {
            (await GetConnection(message)).OnIceCandidate(message.Payload);
        }

        public async ValueTask SetDescription(IServiceMessage<RTCSessionDescription> message)
        {
            await (await GetConnection(message)).InitializeInfo(message.Payload);
        }

        public async ValueTask RequestVideo(IServiceMessage message)
        {
            if (CurrentScreenShare == null) return;

            var connection = await GetConnection(message);
            connection.AddVideo(CurrentScreenShare.VideoTrack!);
        }

        private async ValueTask<RtcMediaConnection> GetConnection(IServiceMessage message)
        {
            if (_connections.TryGetValue(message.Participant, out var connection))
                return connection;

            connection = await _connectionFactory.Create(message.Participant, message.Context.ConnectionId);
            connection.ScreenShareActivated += ConnectionOnScreenShareActivated;

            var result = _connections.GetOrAdd(message.Participant, connection);
            if (result != connection) connection.Dispose();

            return result;
        }

        private void ConnectionOnScreenShareActivated(object? sender, RtcMediaConnection e)
        {
            CurrentScreenShare = e;

            _synchronizedMedia.Update(new SynchronizedMedia(true, e.Participant.ParticipantId));
        }
    }
}