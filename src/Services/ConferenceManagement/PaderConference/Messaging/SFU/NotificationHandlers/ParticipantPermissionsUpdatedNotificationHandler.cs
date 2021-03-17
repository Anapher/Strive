using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PaderConference.Core.Services.Permissions.Notifications;
using PaderConference.Messaging.SFU.Dto;
using PaderConference.Messaging.SFU.Utils;

namespace PaderConference.Messaging.SFU.NotificationHandlers
{
    public class
        ParticipantPermissionsUpdatedNotificationHandler : INotificationHandler<
            ParticipantPermissionsUpdatedNotification>
    {
        private readonly ISfuNotifier _notifier;

        public ParticipantPermissionsUpdatedNotificationHandler(ISfuNotifier notifier)
        {
            _notifier = notifier;
        }

        public async Task Handle(ParticipantPermissionsUpdatedNotification notification,
            CancellationToken cancellationToken)
        {
            var updatedPermissions = new Dictionary<string, SfuParticipantPermissions>();

            foreach (var (participant, permissions) in notification.UpdatedPermissions)
            {
                var sfuPermissions = await SfuPermissionUtils.MapToSfuPermissions(permissions);
                updatedPermissions.Add(participant.Id, sfuPermissions);
            }

            var conferenceId = notification.UpdatedPermissions.First().Key.ConferenceId;
            var updated = new SfuConferenceInfoUpdate(ImmutableDictionary<string, string>.Empty, updatedPermissions,
                ImmutableList<string>.Empty);

            await _notifier.Update(conferenceId, updated);
        }
    }
}
