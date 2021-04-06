using System.Threading.Tasks;

namespace Strive.Core.Services.Media.Gateways
{
    public interface ISfuAuthTokenFactory
    {
        ValueTask<string> GenerateToken(Participant participant, string connectionId);
    }
}
