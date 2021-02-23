using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PaderConference.Core.Services.ConferenceControl.Notifications;
using PaderConference.Core.Services.ParticipantsList.Gateways;
using PaderConference.Core.Services.Synchronization.Requests;

namespace PaderConference.Core.Services.ParticipantsList.NotificationHandlers
{
    public class ParticipantJoinedNotificationHandler : INotificationHandler<ParticipantJoinedNotification>
    {
        private readonly IParticipantMetadataRepository _participantMetadataRepository;
        private readonly IMediator _mediator;

        public ParticipantJoinedNotificationHandler(IParticipantMetadataRepository participantMetadataRepository,
            IMediator mediator)
        {
            _participantMetadataRepository = participantMetadataRepository;
            _mediator = mediator;
        }

        public async Task Handle(ParticipantJoinedNotification notification, CancellationToken cancellationToken)
        {
            var (participant, meta) = notification;

            await _participantMetadataRepository.AddParticipant(participant, meta);
            await _mediator.Send(
                new UpdateSynchronizedObjectRequest(participant.ConferenceId,
                    SynchronizedParticipantsProvider.SynchronizedObjectId), cancellationToken);
        }
    }
}
