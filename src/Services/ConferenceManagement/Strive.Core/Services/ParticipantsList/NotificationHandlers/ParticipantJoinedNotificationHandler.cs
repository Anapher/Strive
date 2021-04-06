using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Strive.Core.Services.ConferenceControl.Notifications;
using Strive.Core.Services.ParticipantsList.Gateways;
using Strive.Core.Services.Synchronization.Requests;

namespace Strive.Core.Services.ParticipantsList.NotificationHandlers
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
                new UpdateSynchronizedObjectRequest(participant.ConferenceId, SynchronizedParticipants.SyncObjId),
                cancellationToken);
        }
    }
}
