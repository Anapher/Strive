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
        private readonly ILogger<SetTemporaryPermissionHandler> _logger;

        public SetTemporaryPermissionHandler(IMediator mediator,
            ITemporaryPermissionRepository temporaryPermissionRepository,
            IJoinedParticipantsRepository participantsRepository, ILogger<SetTemporaryPermissionHandler> logger)
        {
            _mediator = mediator;
            _temporaryPermissionRepository = temporaryPermissionRepository;
            _participantsRepository = participantsRepository;
            _logger = logger;
        }

        public async Task<SuccessOrError<Unit>> Handle(SetTemporaryPermissionRequest request,
            CancellationToken cancellationToken)
        {
            var (targetParticipantId, permissionKey, value, conferenceId) = request;

            _logger.LogDebug("Set temporary permission \"{permissionKey}\" of participant {participantId} to {value}",
                permissionKey, targetParticipantId, value);

            if (!DefinedPermissionsProvider.All.TryGetValue(permissionKey, out var descriptor))
                return PermissionsError.PermissionKeyNotFound(permissionKey);

            if (value != null)
            {
                if (!descriptor.ValidateValue(value))
                    return PermissionsError.InvalidPermissionValueType;

                await _temporaryPermissionRepository.SetTemporaryPermission(conferenceId, targetParticipantId,
                    descriptor.Key, value);

                if (!await _participantsRepository.IsParticipantJoined(conferenceId, targetParticipantId))
                {
                    await _temporaryPermissionRepository.RemoveAllTemporaryPermissions(conferenceId,
                        targetParticipantId);
                    return SuccessOrError<Unit>.Succeeded(Unit.Value);
                }
            }
            else
            {
                await _temporaryPermissionRepository.RemoveTemporaryPermission(conferenceId, targetParticipantId,
                    descriptor.Key);
            }

            await _mediator.Send(new UpdateParticipantsPermissionsRequest(conferenceId, new[] {targetParticipantId}));
            return Unit.Value;
        }
    }
}
