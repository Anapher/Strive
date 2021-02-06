using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PaderConference.Core.Domain.Entities;
using PaderConference.Core.Interfaces.Gateways.Repositories;

namespace PaderConference.Core.Services.Permissions.PermissionLayers
{
    public class ConferencePermissionLayerProvider : IPermissionLayerProvider
    {
        private readonly IConferenceRepo _conferenceRepo;

        public ConferencePermissionLayerProvider(IConferenceRepo conferenceRepo)
        {
            _conferenceRepo = conferenceRepo;
        }

        public async ValueTask<IEnumerable<PermissionLayer>> FetchPermissionsForParticipant(string conferenceId,
            string participantId)
        {
            var conference = await _conferenceRepo.FindById(conferenceId);
            if (conference == null) throw new ConferenceNotFoundException(conferenceId);

            var result = new List<PermissionLayer>
            {
                CommonPermissionLayers.Conference(conference.Permissions[PermissionType.Conference]),
            };

            if (conference.Configuration.Moderators.Contains(participantId))
                result.Add(CommonPermissionLayers.Moderator(conference.Permissions[PermissionType.Moderator]));

            return result;
        }
    }
}
