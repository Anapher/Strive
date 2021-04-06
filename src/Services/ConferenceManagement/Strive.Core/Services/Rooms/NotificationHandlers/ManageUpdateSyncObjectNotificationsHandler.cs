using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Strive.Core.Services.Rooms.Notifications;
using Strive.Core.Services.Rooms.Notifications.Base;
using Strive.Core.Services.Synchronization.Requests;

namespace Strive.Core.Services.Rooms.NotificationHandlers
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
            await _mediator.Send(new UpdateSynchronizedObjectRequest(conferenceId, SynchronizedRooms.SyncObjId));
        }
    }
}
