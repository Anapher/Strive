using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using PaderConference.Core.Services.ConferenceControl.Gateways;
using PaderConference.Core.Services.ConferenceControl.Notifications;
using PaderConference.Core.Services.ConferenceControl.Requests;
using PaderConference.Core.Services.ConferenceManagement.Requests;

namespace PaderConference.Core.Services.ConferenceControl.UseCases
{
    public class OpenConferenceHandler : IRequestHandler<OpenConferenceRequest>
    {
        private readonly ILogger<OpenConferenceHandler> _logger;
        private readonly IMediator _mediator;
        private readonly IOpenConferenceRepository _openConferenceRepository;

        public OpenConferenceHandler(IOpenConferenceRepository openConferenceRepository, IMediator mediator,
            ILogger<OpenConferenceHandler> logger)
        {
            _openConferenceRepository = openConferenceRepository;
            _mediator = mediator;
            _logger = logger;
        }

        public async Task<Unit> Handle(OpenConferenceRequest request, CancellationToken cancellationToken)
        {
            var conferenceId = request.ConferenceId;
            _logger.LogDebug("Attempt to open conference {conferenceId}", conferenceId);

            await VerifyConferenceExists(conferenceId);

            if (await TryOpenConference(request.ConferenceId))
            {
                _logger.LogDebug("Conference opened");
                await _mediator.Publish(new ConferenceOpenedNotification(conferenceId));
            }
            else
            {
                _logger.LogDebug("The conference is already open.");
            }

            return Unit.Value;
        }

        private async Task VerifyConferenceExists(string conferenceId)
        {
            try
            {
                await _mediator.Send(new FindConferenceByIdRequest(conferenceId));
            }
            catch (ConferenceNotFoundException)
            {
                _logger.LogDebug("Conference was not found in database");
                throw;
            }
        }

        private Task<bool> TryOpenConference(string conferenceId)
        {
            return _openConferenceRepository.Create(conferenceId);
        }
    }
}
