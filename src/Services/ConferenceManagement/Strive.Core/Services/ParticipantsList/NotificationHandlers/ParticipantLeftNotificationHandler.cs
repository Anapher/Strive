using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Strive.Core.Services.ConferenceControl.Notifications;
using Strive.Core.Services.ParticipantsList.Gateways;
using Strive.Core.Services.Synchronization.Requests;

namespace Strive.Core.Services.ParticipantsList.NotificationHandlers
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
                new UpdateSynchronizedObjectRequest(participant.ConferenceId, SynchronizedParticipants.SyncObjId),
                cancellationToken);
        }
    }
}
