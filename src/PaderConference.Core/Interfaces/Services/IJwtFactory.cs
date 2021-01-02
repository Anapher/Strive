using System.Threading.Tasks;

namespace PaderConference.Core.Interfaces.Services
{
    public interface IJwtFactory
    {
        ValueTask<string> GenerateModeratorToken(string id, string name);

        ValueTask<string> GenerateGuestToken(string name, string id);
    }
}