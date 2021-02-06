using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PaderConference.Core.Interfaces;
using PaderConference.Core.Services.Permissions.Dto;
using PaderConference.Core.Services.Permissions.Requests;

namespace PaderConference.Core.Services.Permissions.UseCases
{
    public class
        FetchPermissionsHandler : IRequestHandler<FetchPermissionsRequest, SuccessOrError<ParticipantPermissionDto>>
    {
        private readonly IPermissionLayersAggregator _permissionLayersAggregator;

        public FetchPermissionsHandler(IPermissionLayersAggregator permissionLayersAggregator)
        {
            _permissionLayersAggregator = permissionLayersAggregator;
        }

        public async Task<SuccessOrError<ParticipantPermissionDto>> Handle(FetchPermissionsRequest request,
            CancellationToken cancellationToken)
        {
            var (participantId, conferenceId) = request;

            var permissions =
                await _permissionLayersAggregator.FetchParticipantPermissionLayers(conferenceId, participantId);
            return new ParticipantPermissionDto(participantId, permissions);
        }
    }
}
