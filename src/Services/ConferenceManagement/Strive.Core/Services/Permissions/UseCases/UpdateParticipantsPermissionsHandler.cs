using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Strive.Core.Services.ConferenceControl.Gateways;
using Strive.Core.Services.Permissions.Gateways;
using Strive.Core.Services.Permissions.Notifications;
using Strive.Core.Services.Permissions.Requests;
using PermissionsDict = System.Collections.Generic.Dictionary<string, Newtonsoft.Json.Linq.JValue>;

namespace Strive.Core.Services.Permissions.UseCases
{
    public class UpdateParticipantsPermissionsHandler : IRequestHandler<UpdateParticipantsPermissionsRequest>
    {
        private readonly IAggregatedPermissionRepository _permissionRepository;
        private readonly IJoinedParticipantsRepository _joinedParticipantsRepository;
        private readonly IPermissionLayersAggregator _permissionLayersAggregator;
        private readonly IMediator _mediator;
        private readonly ILogger<UpdateParticipantsPermissionsHandler> _logger;

        public UpdateParticipantsPermissionsHandler(IAggregatedPermissionRepository permissionRepository,
            IJoinedParticipantsRepository joinedParticipantsRepository,
            IPermissionLayersAggregator permissionLayersAggregator, IMediator mediator,
            ILogger<UpdateParticipantsPermissionsHandler> logger)
        {
            _permissionRepository = permissionRepository;
            _joinedParticipantsRepository = joinedParticipantsRepository;
            _permissionLayersAggregator = permissionLayersAggregator;
            _mediator = mediator;
            _logger = logger;
        }

        public async Task<Unit> Handle(UpdateParticipantsPermissionsRequest request,
            CancellationToken cancellationToken)
        {
            var participants = request.Participants;

            var newPermissions = new Dictionary<Participant, PermissionsDict>();
            foreach (var participant in participants)
            {
                newPermissions.Add(participant,
                    await _permissionLayersAggregator.FetchAggregatedPermissions(participant));

                cancellationToken.ThrowIfCancellationRequested();
            }

            _logger.LogDebug("Update permissions for {count} participants", newPermissions.Count);

            var appliedPermissions = new Dictionary<Participant, PermissionsDict>();
            foreach (var (participant, permissions) in newPermissions)
            {
                await _permissionRepository.SetPermissions(participant, permissions);

                if (await _joinedParticipantsRepository.IsParticipantJoined(participant))
                {
                    appliedPermissions.Add(participant, permissions);
                }
                else
                {
                    _logger.LogDebug("Participant {participantId} is not joined to the conference, remove permissions.",
                        participant);
                    await _permissionRepository.DeletePermissions(participant);
                }
            }

            if (appliedPermissions.Any())
                await _mediator.Publish(new ParticipantPermissionsUpdatedNotification(appliedPermissions));

            return Unit.Value;
        }
    }
}
