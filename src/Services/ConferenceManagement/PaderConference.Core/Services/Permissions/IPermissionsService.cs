using System.Collections.Generic;
using System.Threading.Tasks;
using PaderConference.Core.Domain.Entities;

namespace PaderConference.Core.Services.Permissions
{
    /// <summary>
    ///     The permission service provides the permissions of each participants
    /// </summary>
    public interface IPermissionsService
    {
        /// <summary>
        ///     Get the permissions of a participant
        /// </summary>
        /// <param name="participant">The participant it should fetch the permissions for</param>
        /// <returns>Return the current permissions of the participant.</returns>
        ValueTask<IPermissionStack> GetPermissions(Participant participant);

        /// <summary>
        ///     Register a new permission layer provider that will be called on <see cref="RefreshPermissions" /> for a
        ///     participant. Please note that permissions of this layer change for a participant, you must call
        ///     <see cref="RefreshPermissions" />, else the permissions won't get updated
        /// </summary>
        /// <param name="fetchPermissions">The fetch permissions delegate</param>
        void RegisterLayerProvider(FetchPermissionsDelegate fetchPermissions);

        /// <summary>
        ///     Refresh the permissions of the given participants.
        /// </summary>
        /// <param name="participants">The participants that will get updated</param>
        ValueTask RefreshPermissions(IEnumerable<Participant> participants);
    }
}