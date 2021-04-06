using System.Threading.Tasks;

namespace Strive.Core.Services.ConferenceControl.Gateways
{
    public interface IOpenConferenceRepository
    {
        /// <summary>
        ///     Create a an open conference
        /// </summary>
        /// <param name="conferenceId">The conference id</param>
        /// <returns>Return true if the conference could be created, false if it already existed</returns>
        Task<bool> Create(string conferenceId);

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
        /// <returns>Return true if the conference exists</returns>
        Task<bool> IsOpen(string conferenceId);
    }
}
