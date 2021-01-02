using PaderConference.Core.Domain.Entities;

namespace PaderConference.Core.Interfaces.Services
{
    public interface IRefreshTokenFactory
    {
        RefreshToken Create(string userId);
    }
}
