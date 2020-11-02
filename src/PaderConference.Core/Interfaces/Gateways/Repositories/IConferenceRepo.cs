using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PaderConference.Core.Domain.Entities;

namespace PaderConference.Core.Interfaces.Gateways.Repositories
{
    public interface IConferenceRepo
    {
        Task<Conference?> FindById(string conferenceId);

        Task Create(Conference conference);

        Task Update(Conference conference);

        Task SetConferenceState(string conferenceId, ConferenceState state);

        Task<IReadOnlyList<Conference>> GetActiveConferences();

        Task<Func<Task>> SubscribeConferenceUpdated(string conferenceId, Func<Conference, Task> handler);
    }
}