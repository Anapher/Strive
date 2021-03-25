using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PaderConference.Core.Services.Equipment.Gateways;
using PaderConference.Core.Services.Equipment.Notifications;
using PaderConference.Core.Services.Synchronization.Requests;

namespace PaderConference.Core.Services.Equipment.NotificationHandlers
{
    public class EquipmentDisconnectedNotificationHandler : INotificationHandler<EquipmentDisconnectedNotification>
    {
        private readonly IEquipmentConnectionRepository _repository;
        private readonly IMediator _mediator;

        public EquipmentDisconnectedNotificationHandler(IEquipmentConnectionRepository repository, IMediator mediator)
        {
            _repository = repository;
            _mediator = mediator;
        }

        public async Task Handle(EquipmentDisconnectedNotification notification, CancellationToken cancellationToken)
        {
            await _repository.RemoveConnection(notification.Participant, notification.ConnectionId);
            await _mediator.Send(new UpdateSynchronizedObjectRequest(notification.Participant.ConferenceId,
                SynchronizedEquipmentProvider.GetObjIdOfParticipant(notification.Participant.Id)));
        }
    }
}
