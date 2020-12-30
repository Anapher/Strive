using System;
using System.Threading.Tasks;
using PaderConference.Core.Domain.Entities;

namespace PaderConference.Core.Interfaces.Gateways.Repositories
{
    public interface IConferenceRepo
    {
        Task<Conference?> FindById(string conferenceId);

        Task Create(Conference conference);

        Task Update(Conference conference);

        Task<IAsyncDisposable> SubscribeConferenceUpdated(string conferenceId, Func<Conference, Task> handler);
    }
}