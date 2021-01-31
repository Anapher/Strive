using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PaderConference.Core.Interfaces;
using PaderConference.Core.NewServices.Permissions.Dto;
using PaderConference.Core.NewServices.Permissions.Requests;
using PaderConference.Core.Services;

namespace PaderConference.Core.NewServices.Permissions.UseCases
{
    public class
        FetchPermissionsHandler : IRequestHandler<FetchPermissionsRequest, SuccessOrError<ParticipantPermissionDto>>
    {
        private readonly IPermissionLayersAggregator _permissionLayersAggregator;
        private readonly IParticipantPermissions _participantPermissions;

        public FetchPermissionsHandler(IPermissionLayersAggregator permissionLayersAggregator,
            IParticipantPermissions participantPermissions)
        {
            _permissionLayersAggregator = permissionLayersAggregator;
            _participantPermissions = participantPermissions;
        }

        public async Task<SuccessOrError<ParticipantPermissionDto>> Handle(FetchPermissionsRequest request,
            CancellationToken cancellationToken)
        {
            var (ofParticipant, meta) = request;
            ofParticipant ??= request.Meta.Sender.ParticipantId;

            // you may always fetch your own permissions
            if (ofParticipant != meta.Sender.ParticipantId)
            {
                var myPermissions =
                    await _participantPermissions.FetchForParticipant(meta.ConferenceId, meta.Sender.ParticipantId);

                if (!await myPermissions.GetPermissionValue(DefinedPermissions.Permissions
                    .CanSeeAnyParticipantsPermissions))
                    return CommonError.PermissionDenied(DefinedPermissions.Permissions
                        .CanSeeAnyParticipantsPermissions);
            }

            var permissions =
                await _permissionLayersAggregator.FetchParticipantPermissionLayers(meta.ConferenceId, ofParticipant);
            return new ParticipantPermissionDto(ofParticipant, permissions);
        }
    }
}
