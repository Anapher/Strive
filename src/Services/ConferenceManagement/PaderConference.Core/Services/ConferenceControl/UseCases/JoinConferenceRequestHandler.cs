using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using PaderConference.Core.Services.ConferenceControl.ClientControl;
using PaderConference.Core.Services.ConferenceControl.Gateways;
using PaderConference.Core.Services.ConferenceControl.Notifications;
using PaderConference.Core.Services.ConferenceControl.Requests;

namespace PaderConference.Core.Services.ConferenceControl.UseCases
{
    public class JoinConferenceRequestHandler : IRequestHandler<JoinConferenceRequest>
    {
        private readonly IMediator _mediator;
        private readonly IJoinedParticipantsRepository _joinedParticipantsRepository;
        private readonly ILogger<JoinConferenceRequestHandler> _logger;

        public JoinConferenceRequestHandler(IMediator mediator,
            IJoinedParticipantsRepository joinedParticipantsRepository, ILogger<JoinConferenceRequestHandler> logger)
        {
            _mediator = mediator;
            _joinedParticipantsRepository = joinedParticipantsRepository;
            _logger = logger;
        }

        public async Task<Unit> Handle(JoinConferenceRequest request, CancellationToken cancellationToken)
        {
            var (conferenceId, participantId, connectionId) = request;

            _logger.LogDebug("Participant {participantId} is joining conference {conferenceId}", participantId,
                conferenceId);

            var previousSession = await _joinedParticipantsRepository.AddParticipant(participantId,
                conferenceId, connectionId);

            if (previousSession != null)
            {
                _logger.LogDebug("The participant {participantId} was already joined, kick him.", participantId);
                await _mediator.Publish(new ParticipantKickedNotification(participantId, previousSession.ConferenceId,
                    previousSession.ConnectionId, ParticipantKickedReason.NewSessionConnected));
            }

            // enable messaging just after kicking client

            // do not merge these together as handlers for ParticipantJoinedNotification may want to send messages to the participant
            await _mediator.Send(new EnableParticipantMessagingRequest(participantId, conferenceId, connectionId),
                cancellationToken);

            await _mediator.Publish(new ParticipantJoinedNotification(participantId, conferenceId));

            return Unit.Value;
        }
    }
}
