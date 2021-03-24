using System.Threading.Tasks;
using PaderConference.Core.Interfaces.Gateways;

namespace PaderConference.Core.Services.Equipment.Gateways
{
    public interface IEquipmentTokenRepository : IStateRepository
    {
        ValueTask Set(Participant participant, string token);

        ValueTask<string?> Get(Participant participant);
    }
}
