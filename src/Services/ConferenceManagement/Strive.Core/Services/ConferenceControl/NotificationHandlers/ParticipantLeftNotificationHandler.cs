using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Strive.Core.Services.ConferenceControl.Gateways;
using Strive.Core.Services.ConferenceControl.Notifications;

namespace Strive.Core.Services.ConferenceControl.NotificationHandlers
{
    public class ParticipantLeftNotificationHandler : INotificationHandler<ParticipantLeftNotification>
    {
        private readonly IJoinedParticipantsRepository _joinedParticipantsRepository;

        public ParticipantLeftNotificationHandler(IJoinedParticipantsRepository joinedParticipantsRepository)
        {
            _joinedParticipantsRepository = joinedParticipantsRepository;
        }

        public async Task Handle(ParticipantLeftNotification notification, CancellationToken cancellationToken)
        {
            await _joinedParticipantsRepository.RemoveParticipant(notification.Participant.Id,
                notification.ConnectionId);
        }
    }
}
