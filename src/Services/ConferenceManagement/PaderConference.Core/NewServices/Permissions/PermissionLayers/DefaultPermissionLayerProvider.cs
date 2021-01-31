using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using PaderConference.Core.Domain.Entities;
using PaderConference.Core.Interfaces.Gateways.Repositories;
using PaderConference.Core.NewServices.Permissions.Options;
using PaderConference.Core.Services;

namespace PaderConference.Core.NewServices.Permissions.PermissionLayers
{
    public class DefaultPermissionLayerProvider : IPermissionLayerProvider
    {
        private readonly IConferenceRepo _conferenceRepo;
        private readonly DefaultPermissionOptions _options;

        public DefaultPermissionLayerProvider(IConferenceRepo conferenceRepo,
            IOptionsSnapshot<DefaultPermissionOptions> options)
        {
            _conferenceRepo = conferenceRepo;
            _options = options.Value;
        }

        public async ValueTask<IEnumerable<PermissionLayer>> FetchPermissionsForParticipant(string conferenceId,
            string participantId)
        {
            var result = new List<PermissionLayer>
            {
                CommonPermissionLayers.ConferenceDefault(_options.Default[PermissionType.Conference]),
            };

            var conference = await _conferenceRepo.FindById(conferenceId);
            if (conference == null) throw new ConferenceNotFoundException(conferenceId);

            if (conference.Configuration.Moderators.Contains(participantId))
                result.Add(CommonPermissionLayers.ModeratorDefault(_options.Default[PermissionType.Moderator]));

            return result;
        }
    }
}
