using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Strive.Core.Interfaces.Gateways.Repositories;
using Strive.Core.Services.ConferenceControl.Notifications;
using Strive.Core.Services.Rooms.Requests;

namespace Strive.Core.Services.Rooms.NotificationHandlers
{
    public class ParticipantJoinedNotificationHandler : INotificationHandler<ParticipantJoinedNotification>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ParticipantJoinedNotificationHandler> _logger;

        public ParticipantJoinedNotificationHandler(IMediator mediator,
            ILogger<ParticipantJoinedNotificationHandler> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        public async Task Handle(ParticipantJoinedNotification notification, CancellationToken cancellationToken)
        {
            var participant = notification.Participant;

            try
            {
                await _mediator.Send(new SetParticipantRoomRequest(participant, RoomOptions.DEFAULT_ROOM_ID),
                    cancellationToken);
            }
            catch (ConcurrencyException e)
            {
                _logger.LogDebug(e,
                    "The participant could not be assigned to the default room. Because the default room does not exist, the conference may be closed.");
            }
        }
    }
}
