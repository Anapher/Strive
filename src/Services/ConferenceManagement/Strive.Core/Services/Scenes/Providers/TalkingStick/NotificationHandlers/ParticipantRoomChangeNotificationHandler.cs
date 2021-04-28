using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Strive.Core.Extensions;
using Strive.Core.Services.Rooms.Notifications;
using Strive.Core.Services.Scenes.Providers.TalkingStick.Gateways;
using Strive.Core.Services.Synchronization.Requests;

namespace Strive.Core.Services.Scenes.Providers.TalkingStick.NotificationHandlers
{
    public class ParticipantRoomChangeNotificationHandler : INotificationHandler<ParticipantsRoomChangedNotification>
    {
        private readonly IMediator _mediator;
        private readonly ITalkingStickRepository _repository;
        private readonly ITalkingStickModeHandler _modeHandler;

        public ParticipantRoomChangeNotificationHandler(IMediator mediator, ITalkingStickRepository repository,
            ITalkingStickModeHandler modeHandler)
        {
            _mediator = mediator;
            _repository = repository;
            _modeHandler = modeHandler;
        }

        public async Task Handle(ParticipantsRoomChangedNotification notification, CancellationToken cancellationToken)
        {
            var updatedRooms = GetRoomsWithAParticipantLeft(notification);
            foreach (var updatedRoom in updatedRooms)
            {
                await using (await _repository.LockRoom(notification.ConferenceId, updatedRoom))
                {
                    var participants = GetParticipantsThatLeftRoom(notification, updatedRoom);

                    await _repository.RemoveFromQueue(participants, updatedRoom);
                    await _modeHandler.InvalidateTalkingSceneWithLockAcquired(notification.ConferenceId, updatedRoom);
                }

                await _mediator.Send(new UpdateSynchronizedObjectRequest(notification.ConferenceId,
                    SynchronizedSceneTalkingStick.SyncObjId(updatedRoom)));
            }
        }

        private static IEnumerable<string> GetRoomsWithAParticipantLeft(
            ParticipantsRoomChangedNotification notification)
        {
            return notification.Participants.Select(x => x.Value.SourceRoom).WhereNotNull().Distinct();
        }

        private static IEnumerable<Participant> GetParticipantsThatLeftRoom(
            ParticipantsRoomChangedNotification notification, string roomId)
        {
            return notification.Participants.Where(x => x.Value.SourceRoom == roomId).Select(x => x.Key);
        }
    }
}
