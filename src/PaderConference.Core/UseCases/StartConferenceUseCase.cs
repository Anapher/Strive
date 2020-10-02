using System.Threading.Tasks;
using PaderConference.Core.Dto.UseCaseRequests;
using PaderConference.Core.Dto.UseCaseResponses;
using PaderConference.Core.Interfaces;
using PaderConference.Core.Interfaces.Services;
using PaderConference.Core.Interfaces.UseCases;

namespace PaderConference.Core.UseCases
{
    public class StartConferenceUseCase : UseCaseStatus<StartConferenceResponse>, IStartConferenceUseCase
    {
        private readonly IConferenceManager _conferenceManager;

        public StartConferenceUseCase(IConferenceManager conferenceManager)
        {
            _conferenceManager = conferenceManager;
        }

        public async ValueTask<StartConferenceResponse?> Handle(StartConferenceRequest message)
        {
            var conference = await _conferenceManager.CreateConference(message.UserId, null);
            return new StartConferenceResponse(conference.ConferenceId);
        }
    }
}