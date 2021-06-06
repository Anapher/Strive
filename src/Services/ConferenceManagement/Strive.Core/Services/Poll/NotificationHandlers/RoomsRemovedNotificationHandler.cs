using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Strive.Core.Services.Poll.Gateways;
using Strive.Core.Services.Poll.Requests;
using Strive.Core.Services.Rooms.Notifications;

namespace Strive.Core.Services.Poll.NotificationHandlers
{
    public class RoomsRemovedNotificationHandler : INotificationHandler<RoomsRemovedNotification>
    {
        private readonly IPollRepository _repository;
        private readonly IMediator _mediator;

        public RoomsRemovedNotificationHandler(IPollRepository repository, IMediator mediator)
        {
            _repository = repository;
            _mediator = mediator;
        }

        public async Task Handle(RoomsRemovedNotification notification, CancellationToken cancellationToken)
        {
            var polls = await _repository.GetPollsOfConference(notification.ConferenceId);

            foreach (var poll in polls.Where(x => x.RoomId != null && notification.RemovedRoomIds.Contains(x.RoomId)))
            {
                await _mediator.Send(new DeletePollRequest(notification.ConferenceId, poll.Id));
            }
        }
    }
}
