using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using PaderConference.Core.Domain.Entities;

namespace PaderConference.Hubs.Media
{
    public interface IRtcMediaConnectionFactory
    {
        ValueTask<RtcMediaConnection> Create(Participant participant, string connectionId);
    }

    public class RtcMediaConnectionFactory : IRtcMediaConnectionFactory
    {
        private readonly IHubContext<CoreHub> _hubContext;
        private readonly ILogger<RtcMediaConnection> _logger;

        public RtcMediaConnectionFactory(IHubContext<CoreHub> hubContext, ILogger<RtcMediaConnection> logger)
        {
            _hubContext = hubContext;
            _logger = logger;
        }

        public async ValueTask<RtcMediaConnection> Create(Participant participant, string connectionId)
        {
            var connection = new RtcMediaConnection(participant, _hubContext.Clients.Client(connectionId), _logger);
            await connection.Init();

            return connection;
        }
    }
}