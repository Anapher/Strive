using System.Threading.Tasks;

namespace PaderConference.Core.Services.Media.Gateways
{
    public interface ISfuAuthTokenFactory
    {
        ValueTask<string> GenerateToken(Participant participant, string connectionId);
    }
}
