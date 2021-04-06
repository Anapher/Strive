using System.Threading.Tasks;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using Strive.Hubs.Core;
using Strive.Messaging.SFU.ReceiveContracts;

namespace Strive.Messaging.Consumers
{
    public class NotifyConnectionConsumer : IConsumer<SendMessageToConnection>
    {
        private readonly IHubContext<CoreHub> _hubContext;

        public NotifyConnectionConsumer(IHubContext<CoreHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task Consume(ConsumeContext<SendMessageToConnection> context)
        {
            var message = context.Message;
            await _hubContext.Clients.Client(message.ConnectionId).SendAsync(message.MethodName, message.Payload);
        }
    }
}
