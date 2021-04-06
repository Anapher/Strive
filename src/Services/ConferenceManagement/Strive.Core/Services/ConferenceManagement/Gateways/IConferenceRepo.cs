using System.Threading.Tasks;
using Strive.Core.Domain.Entities;
using Strive.Core.Interfaces.Gateways;

namespace Strive.Core.Services.ConferenceManagement.Gateways
{
    public interface IConferenceRepo
    {
        Task<Conference?> FindById(string conferenceId);

        Task Create(Conference conference);

        Task<OptimisticUpdateResult> Update(Conference conference);
    }
}