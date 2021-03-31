using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PaderConference.Core.Services.ConferenceControl.Notifications;
using PaderConference.Core.Services.ParticipantsList;
using PaderConference.Core.Services.Synchronization.Requests;
using PaderConference.Messaging.SFU.Dto;

namespace PaderConference.Messaging.SFU.NotificationHandlers
{
    public class ConferenceOpenedNotificationHandler : INotificationHandler<ConferenceOpenedNotification>
    {
        private readonly IMediator _mediator;
        private readonly ISfuConferenceInfoProvider _infoProvider;
        private readonly ISfuNotifier _sfuNotifier;

        public ConferenceOpenedNotificationHandler(IMediator mediator, ISfuConferenceInfoProvider infoProvider,
            ISfuNotifier sfuNotifier)
        {
            _mediator = mediator;
            _infoProvider = infoProvider;
            _sfuNotifier = sfuNotifier;
        }

        public async Task Handle(ConferenceOpenedNotification notification, CancellationToken cancellationToken)
        {
            var syncObj = (SynchronizedParticipants) await _mediator.Send(
                new FetchSynchronizedObjectRequest(notification.ConferenceId,
                    SynchronizedParticipantsProvider.SynchronizedObjectId));

            var permissions = await _infoProvider.GetPermissions(notification.ConferenceId, syncObj.Participants.Keys);
            await _sfuNotifier.Update(notification.ConferenceId,
                new SfuConferenceInfoUpdate(ImmutableDictionary<string, string>.Empty, permissions,
                    ImmutableList<string>.Empty));
        }
    }
}
