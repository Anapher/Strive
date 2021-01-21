using System.Threading.Tasks;
using PaderConference.Core.Domain.Entities;

namespace PaderConference.Core.Interfaces.Gateways.Repositories
{
    public interface IRefreshTokenRepo
    {
        /// <summary>
        ///     Add a refresh token
        /// </summary>
        /// <param name="token">The new refresh token</param>
        Task PushRefreshToken(RefreshToken token);

        /// <summary>
        ///     Try to pop a refresh token from a user
        /// </summary>
        /// <param name="userId">The user id</param>
        /// <param name="refreshToken">The refresh token value</param>
        /// <returns>If the user was not found or the refresh token does not belong to this user, return null</returns>
        Task<RefreshToken?> TryPopRefreshToken(string userId, string refreshToken);
    }
}
