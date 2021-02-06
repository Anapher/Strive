using System.Threading.Tasks;
using PaderConference.Core.Domain.Entities;

namespace PaderConference.Core.Interfaces.Gateways.Repositories
{
    public interface IOpenConferenceRepo
    {
        Task DeleteAll();

        /// <summary>
        ///     Create a an open conference
        /// </summary>
        /// <param name="conference">The conference</param>
        /// <returns>Return true if the conference could be created, false if it already existed</returns>
        Task<bool> Create(Conference conference);

        /// <summary>
        ///     Delete an open conference
        /// </summary>
        /// <param name="conferenceId">The conference id</param>
        /// <returns>Return true if the conference could be deleted</returns>
        Task<bool> Delete(string conferenceId);

        /// <summary>
        ///     Check if the conference id exists
        /// </summary>
        /// <param name="conferenceId">The conference id</param>
        /// <returns></returns>
        Task<bool> Exists(string conferenceId);
    }
}
