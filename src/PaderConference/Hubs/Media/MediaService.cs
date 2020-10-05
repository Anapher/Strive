using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using PaderConference.Core.Domain.Entities;

namespace PaderConference.Hubs.Media
{
    public class MediaService : IConferenceService
    {
        private readonly Conference _conference;
        private readonly IRtcMediaConnectionFactory _connectionFactory;

        private readonly ConcurrentDictionary<Participant, RtcMediaConnection> _connections =
            new ConcurrentDictionary<Participant, RtcMediaConnection>();

        private readonly IHubContext<CoreHub> _hubContext;

        public MediaService(Conference conference, IHubContext<CoreHub> hubContext,
            IRtcMediaConnectionFactory connectionFactory)
        {
            _conference = conference;
            _hubContext = hubContext;
            _connectionFactory = connectionFactory;
        }

        public RtcMediaConnection? CurrentScreenShare { get; private set; }

        public ValueTask DisposeAsync()
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

        public ValueTask OnClientDisconnected(Participant participant)
        {
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

            _hubContext.Clients.Group(_conference.ConferenceId).SendAsync("OnScreenShareActivated");
        }
    }
}