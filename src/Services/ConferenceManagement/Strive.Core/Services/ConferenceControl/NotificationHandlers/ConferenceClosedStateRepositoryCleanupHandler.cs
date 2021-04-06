using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Strive.Core.Interfaces.Gateways;
using Strive.Core.Services.ConferenceControl.Notifications;

namespace Strive.Core.Services.ConferenceControl.NotificationHandlers
{
    public class
        ConferenceClosedStateRepositoryCleanupHandler : INotificationHandler<FinalizeConferenceCleanupNotification>
    {
        private readonly IEnumerable<IStateRepository> _repositories;

        public ConferenceClosedStateRepositoryCleanupHandler(IEnumerable<IStateRepository> repositories)
        {
            _repositories = repositories;
        }

        public async Task Handle(FinalizeConferenceCleanupNotification notification,
            CancellationToken cancellationToken)
        {
            foreach (var stateRepository in _repositories)
            {
                await stateRepository.RemoveAllDataOfConference(notification.ConferenceId);
            }
        }
    }
}
