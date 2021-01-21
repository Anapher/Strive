using System.Threading.Tasks;

namespace PaderConference.Core.Interfaces.Services
{
    public interface IAuthService
    {
        ValueTask<IAuthUser?> FindUser(string username);
        ValueTask<IAuthUser?> FindUserById(string id);
    }
}
