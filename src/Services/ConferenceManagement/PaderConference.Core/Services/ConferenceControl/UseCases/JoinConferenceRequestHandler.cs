using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using PaderConference.Core.Services.ConferenceControl.Gateways;
using PaderConference.Core.Services.ConferenceControl.Notifications;
using PaderConference.Core.Services.ConferenceControl.Requests;

namespace PaderConference.Core.Services.ConferenceControl.UseCases
{
    public class JoinConferenceRequestHandler : IRequestHandler<JoinConferenceRequest>
    {
        private readonly IMediator _mediator;
        private readonly IJoinedParticipantsRepository _joinedParticipantsRepository;

        public JoinConferenceRequestHandler(IMediator mediator,
            IJoinedParticipantsRepository joinedParticipantsRepository, ILogger<JoinConferenceRequestHandler> logger)
        {
            _mediator = mediator;
            _joinedParticipantsRepository = joinedParticipantsRepository;
        }

        public async Task<Unit> Handle(JoinConferenceRequest request, CancellationToken cancellationToken)
        {
            var data = new JoinedParticipantData {JoinedAt = DateTimeOffset.UtcNow};
            var previousConferenceId =
                await _joinedParticipantsRepository.RegisterParticipant(request.ParticipantId, request.ConferenceId,
                    data);

            if (previousConferenceId != null)
                await _mediator.Publish(new ParticipantKickedNotification(request.ParticipantId, previousConferenceId));

            await _mediator.Publish(new ParticipantJoinedNotification(request.ParticipantId, request.ConferenceId));

            return Unit.Value;
        }
    }
}
