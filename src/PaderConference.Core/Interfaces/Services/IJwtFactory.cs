using System.Threading.Tasks;

namespace PaderConference.Core.Interfaces.Services
{
    public interface IJwtFactory
    {
        ValueTask<string> GenerateModeratorToken(string id, string email, string name);
        ValueTask<string> GenerateUserToken(string name);
    }
}