using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PaderConference.Core.Services.ConferenceControl.Notifications;
using PaderConference.Core.Services.ParticipantsList.Gateways;
using PaderConference.Core.Services.Synchronization.Requests;

namespace PaderConference.Core.Services.ParticipantsList.NotificationHandlers
{
    public class ParticipantLeftNotificationHandler : INotificationHandler<ParticipantLeftNotification>
    {
        private readonly IParticipantMetadataRepository _participantMetadataRepository;
        private readonly IMediator _mediator;

        public ParticipantLeftNotificationHandler(IParticipantMetadataRepository participantMetadataRepository,
            IMediator mediator)
        {
            _participantMetadataRepository = participantMetadataRepository;
            _mediator = mediator;
        }

        public async Task Handle(ParticipantLeftNotification notification, CancellationToken cancellationToken)
        {
            var (participant, _) = notification;

            await _participantMetadataRepository.RemoveParticipant(participant);
            await _mediator.Send(
                new UpdateSynchronizedObjectRequest(participant.ConferenceId,
                    SynchronizedParticipantsProvider.SynchronizedObjectId), cancellationToken);
        }
    }
}
