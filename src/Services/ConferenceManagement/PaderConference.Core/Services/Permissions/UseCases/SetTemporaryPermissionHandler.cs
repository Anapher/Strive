using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using PaderConference.Core.Extensions;
using PaderConference.Core.Interfaces;
using PaderConference.Core.Services.ConferenceControl.Gateways;
using PaderConference.Core.Services.Permissions.Gateways;
using PaderConference.Core.Services.Permissions.Requests;

namespace PaderConference.Core.Services.Permissions.UseCases
{
    public class SetTemporaryPermissionHandler : IRequestHandler<SetTemporaryPermissionRequest, SuccessOrError<Unit>>
    {
        private readonly IMediator _mediator;
        private readonly ITemporaryPermissionRepository _temporaryPermissionRepository;
        private readonly IJoinedParticipantsRepository _participantsRepository;
        private readonly IPermissionValidator _permissionValidator;
        private readonly ILogger<SetTemporaryPermissionHandler> _logger;

        public SetTemporaryPermissionHandler(IMediator mediator,
            ITemporaryPermissionRepository temporaryPermissionRepository,
            IJoinedParticipantsRepository participantsRepository, IPermissionValidator permissionValidator,
            ILogger<SetTemporaryPermissionHandler> logger)
        {
            _mediator = mediator;
            _temporaryPermissionRepository = temporaryPermissionRepository;
            _participantsRepository = participantsRepository;
            _permissionValidator = permissionValidator;
            _logger = logger;
        }

        public async Task<SuccessOrError<Unit>> Handle(SetTemporaryPermissionRequest request,
            CancellationToken cancellationToken)
        {
            var (participantId, permissionKey, value, conferenceId) = request;

            _logger.LogDebug("Set temporary permission {permissionKey} of participant {participantId} to {value}",
                permissionKey, participantId,
                value?.ToString()); // serilog serialized JValue as an empty enumerable... {$value} does sadly not work for some reason

            if (!_permissionValidator.TryGetDescriptor(permissionKey, out var descriptor))
                return PermissionsError.PermissionKeyNotFound(permissionKey);

            if (value != null)
            {
                if (!descriptor.ValidateValue(value))
                    return PermissionsError.InvalidPermissionValueType;

                await _temporaryPermissionRepository.SetTemporaryPermission(conferenceId, participantId, descriptor.Key,
                    value);

                if (!await _participantsRepository.IsParticipantJoined(conferenceId, participantId))
                {
                    _logger.LogDebug(
                        "After setting temporary permissions for participant {participantId}, it was noticed that the participant is not currently joined. Remove temporary permission.",
                        participantId);

                    await _temporaryPermissionRepository.RemoveAllTemporaryPermissions(conferenceId, participantId);
                    return CommonError.ConcurrencyError;
                }
            }
            else
            {
                await _temporaryPermissionRepository.RemoveTemporaryPermission(conferenceId, participantId,
                    descriptor.Key);
            }

            await _mediator.Send(new UpdateParticipantsPermissionsRequest(conferenceId, new[] {participantId}));
            return Unit.Value;
        }
    }
}
