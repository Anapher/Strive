using System.Threading.Tasks;

namespace PaderConference.Core.Interfaces.Gateways
{
    public interface IStateRepository
    {
        ValueTask RemoveAllDataOfConference(string conferenceId);
    }
}
