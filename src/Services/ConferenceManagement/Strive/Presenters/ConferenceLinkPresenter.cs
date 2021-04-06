using System.Collections.Generic;
using System.Threading.Tasks;
using Strive.Core.Interfaces.Gateways.Repositories;
using Strive.Core.Services.ConferenceControl;
using Strive.Core.Services.ConferenceControl.Gateways;
using Strive.Core.Specifications;
using Strive.Models.Response;

namespace Strive.Presenters
{
    public interface IConferenceLinkPresenter
    {
        Task<IReadOnlyList<ConferenceLinkDto>> GetConferenceLinks(string participantId);
    }

    public class ConferenceLinkPresenter : IConferenceLinkPresenter
    {
        private readonly IConferenceLinkRepo _conferenceLinkRepo;
        private readonly IConferenceScheduler _scheduler;
        private readonly IOpenConferenceRepository _openConferenceRepository;

        public ConferenceLinkPresenter(IConferenceLinkRepo conferenceLinkRepo, IConferenceScheduler scheduler,
            IOpenConferenceRepository openConferenceRepository)
        {
            _conferenceLinkRepo = conferenceLinkRepo;
            _scheduler = scheduler;
            _openConferenceRepository = openConferenceRepository;
        }

        public async Task<IReadOnlyList<ConferenceLinkDto>> GetConferenceLinks(string participantId)
        {
            var result = new List<ConferenceLinkDto>();

            var links = await _conferenceLinkRepo.FindAsync(new ConferenceLinkByParticipant(participantId));
            foreach (var conferenceLink in links)
            {
                var isActive = await _openConferenceRepository.IsOpen(conferenceLink.ConferenceId);
                var scheduled = _scheduler.GetNextExecution(conferenceLink);
                result.Add(new ConferenceLinkDto(conferenceLink, isActive, scheduled));
            }

            return result;
        }
    }
}
