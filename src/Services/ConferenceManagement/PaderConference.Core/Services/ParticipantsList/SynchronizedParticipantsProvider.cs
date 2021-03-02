using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using PaderConference.Core.Domain.Entities;
using PaderConference.Core.Services.ConferenceControl;
using PaderConference.Core.Services.ConferenceManagement.Requests;
using PaderConference.Core.Services.ParticipantsList.Gateways;
using PaderConference.Core.Services.Synchronization;

namespace PaderConference.Core.Services.ParticipantsList
{
    public class SynchronizedParticipantsProvider : SynchronizedObjectProviderForAll<SynchronizedParticipants>
    {
        private readonly IMediator _mediator;
        private readonly IParticipantMetadataRepository _participantMetadataRepository;

        public SynchronizedParticipantsProvider(IMediator mediator,
            IParticipantMetadataRepository participantMetadataRepository)
        {
            _mediator = mediator;
            _participantMetadataRepository = participantMetadataRepository;
        }

        public static SynchronizedObjectId SynchronizedObjectId = new(SynchronizedObjectIds.PARTICIPANTS);

        public override string Id { get; } = SynchronizedObjectId.Id;

        protected override async ValueTask<SynchronizedParticipants> InternalFetchValue(string conferenceId)
        {
            var conference = await _mediator.Send(new FindConferenceByIdRequest(conferenceId));

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

