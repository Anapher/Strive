using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Strive.Core.Services.ConferenceControl.Gateways;
using Strive.Core.Services.ConferenceControl.Notifications;
using Strive.Core.Services.ConferenceControl.Requests;

namespace Strive.Core.Services.ConferenceControl.UseCases
{
    public class CloseConferenceHandler : IRequestHandler<CloseConferenceRequest>
    {
        private readonly IOpenConferenceRepository _openConferenceRepository;
        private readonly IMediator _mediator;
        private readonly ILogger<CloseConferenceHandler> _logger;

        public CloseConferenceHandler(IOpenConferenceRepository openConferenceRepository, IMediator mediator,
            ILogger<CloseConferenceHandler> logger)
        {
            _openConferenceRepository = openConferenceRepository;
            _mediator = mediator;
            _logger = logger;
        }

        public async Task<Unit> Handle(CloseConferenceRequest request, CancellationToken cancellationToken)
        {
            var conferenceId = request.ConferenceId;

            _logger.LogDebug("Attempt to close conference {conferenceId}", conferenceId);
            if (await _openConferenceRepository.Delete(conferenceId))
            {
                await _mediator.Publish(new ConferenceClosedNotification(conferenceId));
                await _mediator.Publish(new FinalizeConferenceCleanupNotification(conferenceId));
                _logger.LogDebug("Conference was closed successfully");
            }
            else
            {
                _logger.LogDebug("Conference was already closed");
            }

            return Unit.Value;
        }
    }
}
