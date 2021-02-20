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

        public async ValueTask<IEnumerable<PermissionLayer>> FetchPermissionsForParticipant(Participant participant)
        {
            var (conferenceId, participantId) = participant;

            var conference = await _conferenceRepo.FindById(conferenceId);
            if (conference == null) throw new ConferenceNotFoundException(conferenceId);

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
