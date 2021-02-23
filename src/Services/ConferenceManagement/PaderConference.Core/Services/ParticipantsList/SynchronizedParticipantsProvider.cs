using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using PaderConference.Core.Domain.Entities;
using PaderConference.Core.Interfaces.Gateways.Repositories;
using PaderConference.Core.Services.ConferenceControl;
using PaderConference.Core.Services.ParticipantsList.Gateways;
using PaderConference.Core.Services.Synchronization;

namespace PaderConference.Core.Services.ParticipantsList
{
    public class SynchronizedParticipantsProvider : SynchronizedObjectProviderForAll<SynchronizedParticipants>
    {
        private readonly IConferenceRepo _conferenceRepo;
        private readonly IParticipantMetadataRepository _participantMetadataRepository;

        public SynchronizedParticipantsProvider(IConferenceRepo conferenceRepo,
            IParticipantMetadataRepository participantMetadataRepository)
        {
            _conferenceRepo = conferenceRepo;
            _participantMetadataRepository = participantMetadataRepository;
        }

        public static SynchronizedObjectId SynchronizedObjectId = new(SynchronizedObjectIds.PARTICIPANTS);

        public override string Id { get; } = SynchronizedObjectId.Id;

        protected override async ValueTask<SynchronizedParticipants> InternalFetchValue(string conferenceId)
        {
            var conference = await _conferenceRepo.FindById(conferenceId);
            if (conference == null) throw new ConferenceNotFoundException(conferenceId);

            var participants = await _participantMetadataRepository.GetParticipantsOfConference(conferenceId);
            var participantsMap =
                participants.ToImmutableDictionary(x => x.Key, x => CreateParticipantData(x.Key, x.Value, conference));
            return new SynchronizedParticipants(participantsMap);
        }

        private static ParticipantData CreateParticipantData(string participantId, ParticipantMetadata metadata,
            Conference conference)
        {
            var isModerator = conference.Configuration.Moderators.Contains(participantId);
            return new ParticipantData(metadata.DisplayName, isModerator);
        }
    }
}

