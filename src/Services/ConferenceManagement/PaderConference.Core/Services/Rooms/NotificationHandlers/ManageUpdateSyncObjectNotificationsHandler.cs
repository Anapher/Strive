using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PaderConference.Core.Services.Rooms.Notifications;
using PaderConference.Core.Services.Rooms.Notifications.Base;
using PaderConference.Core.Services.Synchronization.Requests;

namespace PaderConference.Core.Services.Rooms.NotificationHandlers
{
    public class ManageUpdateSyncObjectNotificationsHandler : INotificationHandler<RoomsChangedNotificationBase>,
        INotificationHandler<ParticipantsRoomChangedNotification>
    {
        private readonly IMediator _mediator;

        public ManageUpdateSyncObjectNotificationsHandler(IMediator mediator)
        {
            _mediator = mediator;
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
            await _mediator.Send(new UpdateSynchronizedObjectRequest(conferenceId,
                SynchronizedRoomsProvider.SynchronizedObjectId));
        }
    }
}
