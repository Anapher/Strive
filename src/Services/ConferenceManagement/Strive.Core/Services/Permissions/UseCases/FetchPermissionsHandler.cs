using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Strive.Core.Services.Permissions.Requests;
using Strive.Core.Services.Permissions.Responses;

namespace Strive.Core.Services.Permissions.UseCases
{
    public class FetchPermissionsHandler : IRequestHandler<FetchPermissionsRequest, ParticipantPermissionResponse>
    {
        private readonly IPermissionLayersAggregator _permissionLayersAggregator;

        public FetchPermissionsHandler(IPermissionLayersAggregator permissionLayersAggregator)
        {
            _permissionLayersAggregator = permissionLayersAggregator;
        }

        public async Task<ParticipantPermissionResponse> Handle(FetchPermissionsRequest request,
            CancellationToken cancellationToken)
        {
            var participant = request.Participant;

            var permissions = await _permissionLayersAggregator.FetchParticipantPermissionLayers(participant);
            return new ParticipantPermissionResponse(participant.Id, permissions);
        }
    }
}
