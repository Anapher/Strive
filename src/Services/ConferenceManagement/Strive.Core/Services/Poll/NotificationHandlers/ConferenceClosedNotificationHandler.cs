using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Strive.Core.Services.ConferenceControl.Notifications;
using Strive.Core.Services.Poll.Gateways;
using Strive.Core.Services.Poll.Requests;

namespace Strive.Core.Services.Poll.NotificationHandlers
{
    public class ConferenceClosedNotificationHandler : INotificationHandler<ConferenceClosedNotification>
    {
        private readonly IPollRepository _repository;
        private readonly IMediator _mediator;

        public ConferenceClosedNotificationHandler(IPollRepository repository, IMediator mediator)
        {
            _repository = repository;
            _mediator = mediator;
        }

        public async Task Handle(ConferenceClosedNotification notification, CancellationToken cancellationToken)
        {
            var polls = await _repository.GetPollsOfConference(notification.ConferenceId);
            foreach (var poll in polls)
            {
                await _mediator.Send(new DeletePollRequest(notification.ConferenceId, poll.Id));
            }
        }
    }
}
