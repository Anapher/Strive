using System.Collections.Generic;
using System.Threading.Tasks;
using PaderConference.Core.Interfaces.Gateways.Repositories;
using PaderConference.Core.Interfaces.Services;
using PaderConference.Core.Services.ConferenceControl;
using PaderConference.Core.Specifications;
using PaderConference.Models.Response;

namespace PaderConference.Presenters
{
    public interface IConferenceLinkPresenter
    {
        Task<IReadOnlyList<ConferenceLinkDto>> GetConferenceLinks(string participantId);
    }

    public class ConferenceLinkPresenter : IConferenceLinkPresenter
    {
        private readonly IConferenceLinkRepo _conferenceLinkRepo;
        private readonly IConferenceManager _conferenceManager;
        private readonly IConferenceScheduler _scheduler;

        public ConferenceLinkPresenter(IConferenceLinkRepo conferenceLinkRepo, IConferenceManager conferenceManager,
            IConferenceScheduler scheduler)
        {
            _conferenceLinkRepo = conferenceLinkRepo;
            _conferenceManager = conferenceManager;
            _scheduler = scheduler;
        }

        public async Task<IReadOnlyList<ConferenceLinkDto>> GetConferenceLinks(string participantId)
        {
            var result = new List<ConferenceLinkDto>();

            var links = await _conferenceLinkRepo.FindAsync(new ConferenceLinkByParticipant(participantId));
            foreach (var conferenceLink in links)
            {
                var isActive = await _conferenceManager.GetIsConferenceOpen(conferenceLink.ConferenceId);
                var scheduled = _scheduler.GetNextExecution(conferenceLink);
                result.Add(new ConferenceLinkDto(conferenceLink, isActive, scheduled));
            }

            return result;
        }
    }
}
