using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Strive.Contracts;
using Strive.Core.Services.ConferenceControl.Gateways;
using Strive.Core.Services.ConferenceControl.Notifications;
using Strive.Hubs.Core.Responses;

namespace Strive.Hubs.Core.NotificationHandlers
{
    public class ParticipantKickedNotificationHandler : INotificationHandler<ParticipantKickedNotification>
    {
        private readonly IHubContext<CoreHub> _hubContext;
        private readonly IJoinedParticipantsRepository _repository;
        private readonly IMediator _mediator;
        private readonly IPublishEndpoint _publishEndpoint;

        public ParticipantKickedNotificationHandler(IPublishEndpoint publishEndpoint, IHubContext<CoreHub> hubContext,
            IJoinedParticipantsRepository repository, IMediator mediator)
        {
            _publishEndpoint = publishEndpoint;
            _hubContext = hubContext;
            _repository = repository;
            _mediator = mediator;
        }

        public async Task Handle(ParticipantKickedNotification notification, CancellationToken cancellationToken)
        {
            var (participant, connectionId, reason) = notification;

            var targetClient = connectionId != null
                ? _hubContext.Clients.Client(connectionId)
                : _hubContext.Clients.Group(CoreHubGroups.OfParticipant(participant));

            await targetClient.OnRequestDisconnect(new RequestDisconnectDto(reason), cancellationToken);

            if (connectionId != null)
                // it's very important that we publish participant left, as a new participant may be joining right after this and
                // we have to clean up first
                await using (var @lock = await _repository.LockParticipantJoin(participant))
                {
                    await _mediator.Publish(new ParticipantLeftNotification(participant, connectionId),
                        @lock.HandleLostToken);
                }

            await _publishEndpoint.Publish<ParticipantKicked>(notification, cancellationToken);
        }
    }
}
