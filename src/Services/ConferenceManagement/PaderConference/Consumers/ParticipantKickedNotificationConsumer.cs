using System.Threading.Tasks;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using PaderConference.Core.Services.ConferenceControl.Notifications;
using PaderConference.Hubs;
using PaderConference.Hubs.Responses;

namespace PaderConference.Consumers
{
    public class ParticipantKickedNotificationConsumer : IConsumer<ParticipantKickedNotification>
    {
        private readonly IHubContext<CoreHub> _hubContext;

        public ParticipantKickedNotificationConsumer(IHubContext<CoreHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task Consume(ConsumeContext<ParticipantKickedNotification> context)
        {
            var (participantId, _, participantKickedReason) = context.Message;

            await _hubContext.Clients.Client(participantId)
                .OnRequestDisconnect(new RequestDisconnectDto(participantKickedReason), context.CancellationToken);
        }
    }
}
