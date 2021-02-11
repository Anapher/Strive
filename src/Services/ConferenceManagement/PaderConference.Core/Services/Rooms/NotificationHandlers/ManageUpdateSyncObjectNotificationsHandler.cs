using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PaderConference.Core.Services.Rooms.Notifications;
using PaderConference.Core.Services.Rooms.Notifications.Base;
using PaderConference.Core.Services.Synchronization;
using PaderConference.Core.Services.Synchronization.Extensions;

namespace PaderConference.Core.Services.Rooms.NotificationHandlers
{
    public class ManageUpdateSyncObjectNotificationsHandler : INotificationHandler<RoomsChangedNotificationBase>,
        INotificationHandler<ParticipantsRoomChangedNotification>
    {
        private readonly ISynchronizedObjectProvider<SynchronizedRooms> _synchronizedObject;

        public ManageUpdateSyncObjectNotificationsHandler(
            ISynchronizedObjectProvider<SynchronizedRooms> synchronizedObject)
        {
            _synchronizedObject = synchronizedObject;
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
            await _synchronizedObject.UpdateWithInitialValue(conferenceId);
        }
    }
}
