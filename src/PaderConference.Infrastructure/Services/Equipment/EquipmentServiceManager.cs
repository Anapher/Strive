using System.Threading.Tasks;
using PaderConference.Core.Interfaces.Services;

namespace PaderConference.Infrastructure.Services.Equipment
{
    public class EquipmentServiceManager : ConferenceServiceManager<EquipmentService>
    {
        private readonly ITokenFactory _tokenFactory;

        public EquipmentServiceManager(ITokenFactory tokenFactory)
        {
            _tokenFactory = tokenFactory;
        }

        protected override ValueTask<EquipmentService> ServiceFactory(string conferenceId)
        {
            return new ValueTask<EquipmentService>(new EquipmentService(conferenceId, _tokenFactory));
        }
    }
}
