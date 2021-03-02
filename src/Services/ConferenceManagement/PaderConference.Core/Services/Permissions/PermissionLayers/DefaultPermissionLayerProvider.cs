using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Options;
using PaderConference.Core.Domain.Entities;
using PaderConference.Core.Services.ConferenceManagement.Requests;
using PaderConference.Core.Services.Permissions.Options;

namespace PaderConference.Core.Services.Permissions.PermissionLayers
{
    public class DefaultPermissionLayerProvider : IPermissionLayerProvider
    {
        private readonly IMediator _mediator;
        private readonly DefaultPermissionOptions _options;

        public DefaultPermissionLayerProvider(IMediator mediator, IOptions<DefaultPermissionOptions> options)
        {
            _mediator = mediator;
            _options = options.Value;
        }

        public async ValueTask<IEnumerable<PermissionLayer>> FetchPermissionsForParticipant(Participant participant)
        {
            var (conferenceId, participantId) = participant;

            var result = new List<PermissionLayer>
            {
                CommonPermissionLayers.ConferenceDefault(_options.Default[PermissionType.Conference]),
            };

            var conference = await _mediator.Send(new FindConferenceByIdRequest(conferenceId));
            if (conference.Configuration.Moderators.Contains(participantId))
                result.Add(CommonPermissionLayers.ModeratorDefault(_options.Default[PermissionType.Moderator]));

            return result;
        }
    }
}
