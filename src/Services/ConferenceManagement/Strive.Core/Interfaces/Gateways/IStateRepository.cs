using System.Threading.Tasks;

namespace Strive.Core.Interfaces.Gateways
{
    public interface IStateRepository
    {
        ValueTask RemoveAllDataOfConference(string conferenceId);
    }
}
