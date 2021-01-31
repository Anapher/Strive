using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using PaderConference.Core.Extensions;
using PaderConference.Core.Interfaces;
using PaderConference.Core.NewServices.Permissions.Gateways;
using PaderConference.Core.NewServices.Permissions.Requests;
using PaderConference.Core.Services;

namespace PaderConference.Core.NewServices.Permissions.UseCases
{
    public class SetTemporaryPermissionHandler : IRequestHandler<SetTemporaryPermissionRequest, SuccessOrError>
    {
        private readonly IParticipantPermissions _participantPermissions;
        private readonly IMediator _mediator;
        private readonly ITemporaryPermissionRepository _temporaryPermissionRepository;
        private readonly ILogger<SetTemporaryPermissionHandler> _logger;

        public SetTemporaryPermissionHandler(IParticipantPermissions participantPermissions, IMediator mediator,
            ITemporaryPermissionRepository temporaryPermissionRepository, ILogger<SetTemporaryPermissionHandler> logger)
        {
            _participantPermissions = participantPermissions;
            _mediator = mediator;
            _temporaryPermissionRepository = temporaryPermissionRepository;
            _logger = logger;
        }

        public async Task<SuccessOrError> Handle(SetTemporaryPermissionRequest request,
            CancellationToken cancellationToken)
        {
            var (targetParticipantId, permissionKey, value, meta) = request;

            var permissions =
                await _participantPermissions.FetchForParticipant(meta.ConferenceId, meta.Sender.ParticipantId);
            if (!await permissions.GetPermissionValue(DefinedPermissions.Permissions.CanGiveTemporaryPermission))
                return CommonError.PermissionDenied(DefinedPermissions.Permissions.CanGiveTemporaryPermission);

            _logger.LogDebug("Set temporary permission \"{permissionKey}\" of participant {participantId} to {value}",
                permissionKey, targetParticipantId, value);

            if (!DefinedPermissionsProvider.All.TryGetValue(permissionKey, out var descriptor))
                return PermissionsError.PermissionKeyNotFound(permissionKey);

            if (value != null)
            {
                if (!descriptor.ValidateValue(value))
                    return PermissionsError.InvalidPermissionValueType;

                await _temporaryPermissionRepository.SetTemporaryPermission(meta.ConferenceId, targetParticipantId,
                    descriptor.Key, value);
            }
            else
            {
                await _temporaryPermissionRepository.RemoveTemporaryPermission(meta.ConferenceId, targetParticipantId,
                    descriptor.Key);
            }

            await _mediator.Send(
                new UpdateParticipantsPermissionsRequest(meta.ConferenceId, new[] {targetParticipantId}));
            return SuccessOrError.Succeeded;
        }
    }
}
