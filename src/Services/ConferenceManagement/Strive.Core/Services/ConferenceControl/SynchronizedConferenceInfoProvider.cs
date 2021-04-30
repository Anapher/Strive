using System.Collections.Immutable;
using System.Threading.Tasks;
using MediatR;
using Strive.Core.Services.ConferenceControl.Gateways;
using Strive.Core.Services.ConferenceManagement.Requests;
using Strive.Core.Services.Synchronization;

namespace Strive.Core.Services.ConferenceControl
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


        public override string Id { get; } = SynchronizedConferenceInfo.SyncObjId.Id;

        protected override async ValueTask<SynchronizedConferenceInfo> InternalFetchValue(string conferenceId)
        {
            var conference = await _mediator.Send(new FindConferenceByIdRequest(conferenceId));

            var nextDate = _scheduler.GetNextExecution(conference.Configuration);
            var isOpen = await _openConferenceRepository.IsOpen(conferenceId);
            return new SynchronizedConferenceInfo(isOpen, conference.Configuration.Moderators.ToImmutableList(),
                nextDate, conference.Configuration.Name, conference.Configuration.Chat.IsPrivateChatEnabled,
                conference.Configuration.Scenes);
        }
    }
}
