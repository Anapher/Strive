using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PaderConference.Core.Services.ConferenceControl.Gateways;
using PaderConference.Core.Services.ConferenceControl.Notifications;

namespace PaderConference.Core.Services.ConferenceControl.NotificationHandlers
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
