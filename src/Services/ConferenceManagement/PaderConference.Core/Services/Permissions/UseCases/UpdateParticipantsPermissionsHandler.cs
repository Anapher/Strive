using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using PaderConference.Core.Services.ConferenceControl.Gateways;
using PaderConference.Core.Services.Permissions.Gateways;
using PaderConference.Core.Services.Permissions.Notifications;
using PaderConference.Core.Services.Permissions.Requests;
using PermissionsDict = System.Collections.Generic.Dictionary<string, Newtonsoft.Json.Linq.JValue>;

namespace PaderConference.Core.Services.Permissions.UseCases
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
            var (conferenceId, participantIds) = request;

            var newPermissions = new Dictionary<string, PermissionsDict>();
            foreach (var participantId in participantIds)
            {
                newPermissions.Add(participantId,
                    await _permissionLayersAggregator.FetchAggregatedPermissions(conferenceId, participantId));

                cancellationToken.ThrowIfCancellationRequested();
            }

            _logger.LogDebug("Update permissions for {count} participants", newPermissions.Count);

            var appliedPermissions = new Dictionary<string, PermissionsDict>();
            foreach (var (participantId, permissions) in newPermissions)
            {
                await _permissionRepository.SetPermissions(conferenceId, participantId, permissions);

                if (await _joinedParticipantsRepository.IsParticipantJoined(participantId, conferenceId))
                    appliedPermissions.Add(participantId, permissions);
                else
                    await _permissionRepository.DeletePermissions(conferenceId, participantId);
            }

            await _mediator.Publish(new ParticipantPermissionsUpdatedNotification(conferenceId, appliedPermissions));
            return Unit.Value;
        }
    }
}
