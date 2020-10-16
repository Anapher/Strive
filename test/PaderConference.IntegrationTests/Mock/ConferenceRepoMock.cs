using System.Threading.Tasks;
using PaderConference.Core.Domain.Entities;
using PaderConference.Core.Interfaces.Gateways.Repositories;

namespace PaderConference.IntegrationTests.Mock
{
    public class ConferenceRepoMock : IConferenceRepo
    {
        public async Task<Conference?> FindById(string conferenceId)
        {
            return null;
        }

        public Task Create(Conference conference)
        {
            return Task.CompletedTask;
        }

        public Task Update(Conference conference)
        {
            return Task.CompletedTask;
        }

        public Task SetConferenceState(string conferenceId, ConferenceState state)
        {
            return Task.CompletedTask;
        }
    }
}