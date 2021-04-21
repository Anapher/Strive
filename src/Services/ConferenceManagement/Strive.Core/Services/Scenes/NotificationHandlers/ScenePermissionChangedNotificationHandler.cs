using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Strive.Core.Services.Permissions.Requests;
using Strive.Core.Services.Synchronization.Notifications;

namespace Strive.Core.Services.Scenes.NotificationHandlers
{
    public class ScenePermissionChangedNotificationHandler : INotificationHandler<SynchronizedObjectUpdatedNotification>
    {
        private readonly IMediator _mediator;

        public ScenePermissionChangedNotificationHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task Handle(SynchronizedObjectUpdatedNotification notification,
            CancellationToken cancellationToken)
        {
            if (notification.Value is SynchronizedScene newSyncScene)
            {
                if ((notification.PreviousValue as SynchronizedScene)?.CurrentSceneStack.SequenceEqual(newSyncScene
                    .CurrentSceneStack) == true)
                {
                    return;
                }

                await _mediator.Send(new UpdateParticipantsPermissionsRequest(notification.Participants));
            }
        }
    }
}
