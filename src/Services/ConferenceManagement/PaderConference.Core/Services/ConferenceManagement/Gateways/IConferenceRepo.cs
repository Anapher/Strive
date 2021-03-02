using System.Threading.Tasks;
using PaderConference.Core.Domain.Entities;
using PaderConference.Core.Interfaces.Gateways;

namespace PaderConference.Core.Services.ConferenceManagement.Gateways
{
    public interface IConferenceRepo
    {
        Task<Conference?> FindById(string conferenceId);

        Task Create(Conference conference);

        Task<OptimisticUpdateResult> Update(Conference conference);
    }
}