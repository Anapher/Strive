using PaderConference.Core.Domain.Entities;
using PaderConference.Core.Dto.GatewayResponses.Repositories;
using System.Threading.Tasks;

namespace PaderConference.Core.Interfaces.Gateways.Repositories
{
    public interface IUserRepository
    {
        Task<CreateUserResponse> Create(string email, string userName, string password);
        Task<User?> FindByName(string userName);
        Task<bool> CheckPassword(User user, string password);
        Task<User?> FindById(string id);

        Task Update(User entity);
        Task Delete(User entity);
    }
}