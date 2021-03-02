using System.Threading.Tasks;
using MediatR;
using PaderConference.Core.Services.ConferenceControl.Gateways;
using PaderConference.Core.Services.ConferenceManagement.Requests;
using PaderConference.Core.Services.Synchronization;

namespace PaderConference.Core.Services.ConferenceControl
{
    public class SynchronizedConferenceInfoProvider : SynchronizedObjectProviderForAll<SynchronizedConferenceInfo>
    {
        private readonly IMediator _mediator;
        private readonly IConferenceScheduler _scheduler;
        private readonly IOpenConferenceRepository _openConferenceRepository;

        public SynchronizedConferenceInfoProvider(IMediator mediator, IConferenceScheduler scheduler,
            IOpenConferenceRepository openConferenceRepository)
        {
            _mediator = mediator;
            _scheduler = scheduler;
            _openConferenceRepository = openConferenceRepository;
        }

        public static SynchronizedObjectId SynchronizedObjectId { get; } = new(SynchronizedObjectIds.CONFERENCE);

        public override string Id { get; } = SynchronizedObjectIds.CONFERENCE;

        protected override async ValueTask<SynchronizedConferenceInfo> InternalFetchValue(string conferenceId)
        {
            var conference = await _mediator.Send(new FindConferenceByIdRequest(conferenceId));

            var nextDate = _scheduler.GetNextExecution(conference.Configuration);
            var isOpen = await _openConferenceRepository.IsOpen(conferenceId);
            return new SynchronizedConferenceInfo(isOpen, conference.Configuration.Moderators, nextDate);
        }
    }
}
