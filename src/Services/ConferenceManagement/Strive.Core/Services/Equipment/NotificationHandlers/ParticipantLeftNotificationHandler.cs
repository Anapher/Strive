using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Strive.Core.Services.ConferenceControl.Notifications;
using Strive.Core.Services.Equipment.Gateways;

namespace Strive.Core.Services.Equipment.NotificationHandlers
{
    public class ParticipantLeftNotificationHandler : INotificationHandler<ParticipantLeftNotification>
    {
        private readonly IEquipmentConnectionRepository _repository;

        public ParticipantLeftNotificationHandler(IEquipmentConnectionRepository repository)
        {
            _repository = repository;
        }

        public async Task Handle(ParticipantLeftNotification notification, CancellationToken cancellationToken)
        {
            await _repository.RemoveAllConnections(notification.Participant);
        }
    }
}
