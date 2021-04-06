using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Strive.Core.Domain.Entities;
using Strive.Core.Services.ConferenceControl;
using Strive.Core.Services.ConferenceManagement.Requests;
using Strive.Core.Services.ParticipantsList.Gateways;
using Strive.Core.Services.Synchronization;

namespace Strive.Core.Services.ParticipantsList
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


        public override string Id { get; } = SynchronizedParticipants.SyncObjId.Id;

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

