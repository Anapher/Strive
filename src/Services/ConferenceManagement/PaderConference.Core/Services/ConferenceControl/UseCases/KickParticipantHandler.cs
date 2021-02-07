using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PaderConference.Core.Services.ConferenceControl.Notifications;
using PaderConference.Core.Services.ConferenceControl.Requests;

namespace PaderConference.Core.Services.ConferenceControl.UseCases
{
    public class KickParticipantHandler : IRequestHandler<KickParticipantRequest>
    {
        private readonly IMediator _mediator;

        public KickParticipantHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<Unit> Handle(KickParticipantRequest request, CancellationToken cancellationToken)
        {
            await _mediator.Publish(new ParticipantKickedNotification(request.ParticipantId, request.ConferenceId));

            return Unit.Value;
        }
    }
}
