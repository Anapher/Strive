using System.Threading.Tasks;
using Strive.Core.Interfaces.Gateways;

namespace Strive.Core.Services.Equipment.Gateways
{
    public interface IEquipmentTokenRepository : IStateRepository
    {
        ValueTask Set(Participant participant, string token);

        ValueTask<string?> Get(Participant participant);
    }
}
