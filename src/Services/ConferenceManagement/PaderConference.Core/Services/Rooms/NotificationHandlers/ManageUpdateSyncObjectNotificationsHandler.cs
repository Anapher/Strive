using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PaderConference.Core.Services.Rooms.Notifications;
using PaderConference.Core.Services.Rooms.Notifications.Base;
using PaderConference.Core.Services.Synchronization;
using PaderConference.Core.Services.Synchronization.Requests;

namespace PaderConference.Core.Services.Rooms.NotificationHandlers
{
    public class ManageUpdateSyncObjectNotificationsHandler : INotificationHandler<RoomsChangedNotificationBase>,
        INotificationHandler<ParticipantsRoomChangedNotification>
    {
        private readonly IMediator _mediator;
        private readonly IParticipantAggregator _participantAggregator;

        public ManageUpdateSyncObjectNotificationsHandler(IMediator mediator,
            IParticipantAggregator participantAggregator)
        {
            _mediator = mediator;
            _participantAggregator = participantAggregator;
        }

        public Task Handle(RoomsChangedNotificationBase notification, CancellationToken cancellationToken)
        {
            return UpdateSynchronizedObject(notification.ConferenceId);
        }

        public Task Handle(ParticipantsRoomChangedNotification notification, CancellationToken cancellationToken)
        {
            return UpdateSynchronizedObject(notification.ConferenceId);
        }

        private async Task UpdateSynchronizedObject(string conferenceId)
        {
            var participants = await _participantAggregator.OfConference(conferenceId);
            await _mediator.Send(
                UpdateSynchronizedObjectRequest.Create<SynchronizedRoomsProvider>(conferenceId, participants));
        }
    }
}
