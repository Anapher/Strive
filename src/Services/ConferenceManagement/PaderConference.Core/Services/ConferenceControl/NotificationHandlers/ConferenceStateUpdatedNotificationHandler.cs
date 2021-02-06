using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using PaderConference.Core.Services.ConferenceControl.Notifications;
using PaderConference.Core.Services.Synchronization;
using PaderConference.Core.Services.Synchronization.Extensions;

namespace PaderConference.Core.Services.ConferenceControl.NotificationHandlers
{
    public class ConferenceStateUpdatedNotificationHandler : INotificationHandler<ConferenceStateUpdatedNotification>
    {
        private readonly ISynchronizedObjectProvider<SynchronizedConferenceInfo> _synchronizedObjectProvider;
        private readonly ILogger<ConferenceStateUpdatedNotificationHandler> _logger;

        public ConferenceStateUpdatedNotificationHandler(
            ISynchronizedObjectProvider<SynchronizedConferenceInfo> synchronizedObjectProvider,
            ILogger<ConferenceStateUpdatedNotificationHandler> logger)
        {
            _synchronizedObjectProvider = synchronizedObjectProvider;
            _logger = logger;
        }

        public async Task Handle(ConferenceStateUpdatedNotification notification, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Conference {conferenceId} updated, update synchronized object",
                notification.ConferenceId);

            await _synchronizedObjectProvider.UpdateWithInitialValue(notification.ConferenceId);
        }
    }
}
