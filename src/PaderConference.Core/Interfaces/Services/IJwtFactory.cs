using System.Threading.Tasks;

namespace PaderConference.Core.Interfaces.Services
{
    public interface IJwtFactory
    {
        ValueTask<string> GenerateEncodedToken(string id, string email);
    }
}