using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Strive.Core.Services.ConferenceControl.Notifications;
using Strive.Core.Services.ConferenceControl.Requests;

namespace Strive.Core.Services.ConferenceControl.UseCases
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
            // there is not much we can do
            await _mediator.Publish(new ParticipantKickedNotification(request.Participant, null,
                ParticipantKickedReason.ByModerator));
            return Unit.Value;
        }
    }
}
