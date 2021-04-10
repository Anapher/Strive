using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Strive.Core.Domain.Entities;
using Strive.Core.Services.ConferenceManagement.Requests;

namespace Strive.Core.Services.Permissions.PermissionLayers
{
    public class ConferencePermissionLayerProvider : IPermissionLayerProvider
    {
        private readonly IMediator _mediator;

        public ConferencePermissionLayerProvider(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async ValueTask<IEnumerable<PermissionLayer>> FetchPermissionsForParticipant(Participant participant)
        {
            var (conferenceId, participantId) = participant;

            var conference = await _mediator.Send(new FindConferenceByIdRequest(conferenceId));

            var result = new List<PermissionLayer>();
            if (conference.Permissions.TryGetValue(PermissionType.Conference, out var conferencePermissions))
                result.Add(CommonPermissionLayers.Conference(conferencePermissions));

            if (conference.Configuration.Moderators.Contains(participantId) &&
                conference.Permissions.TryGetValue(PermissionType.Moderator, out var moderatorPermissions))
                result.Add(CommonPermissionLayers.Moderator(moderatorPermissions));

            return result;
        }
    }
}
