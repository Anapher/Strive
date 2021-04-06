using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Strive.Core.Extensions;
using Strive.Core.Interfaces;
using Strive.Core.Services.ConferenceControl.Gateways;
using Strive.Core.Services.Permissions.Gateways;
using Strive.Core.Services.Permissions.Requests;

namespace Strive.Core.Services.Permissions.UseCases
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
            var (participant, permissionKey, value) = request;

            _logger.LogDebug("Set temporary permission {permissionKey} of participant {participantId} to {value}",
                permissionKey, participant,
                value?.ToString()); // serilog serialized JValue as an empty enumerable... {$value} does sadly not work for some reason

            if (!_permissionValidator.TryGetDescriptor(permissionKey, out var descriptor))
                return PermissionsError.PermissionKeyNotFound(permissionKey);

            if (value != null)
            {
                if (!descriptor.ValidateValue(value))
                    return PermissionsError.InvalidPermissionValueType;

                await _temporaryPermissionRepository.SetTemporaryPermission(participant, descriptor.Key, value);

                if (!await _participantsRepository.IsParticipantJoined(participant))
                {
                    _logger.LogDebug(
                        "After setting temporary permissions for participant {participantId}, it was noticed that the participant is not currently joined. Remove temporary permission.",
                        participant);

                    await _temporaryPermissionRepository.RemoveAllTemporaryPermissions(participant);
                    return CommonError.ConcurrencyError;
                }
            }
            else
            {
                await _temporaryPermissionRepository.RemoveTemporaryPermission(participant, descriptor.Key);
            }

            await _mediator.Send(new UpdateParticipantsPermissionsRequest(participant.Yield()));
            return Unit.Value;
        }
    }
}
