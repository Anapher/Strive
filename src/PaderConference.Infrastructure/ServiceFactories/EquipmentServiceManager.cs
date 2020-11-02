using Autofac;
using PaderConference.Core.Services.Equipment;

namespace PaderConference.Infrastructure.Services.Equipment
{
    public class EquipmentServiceManager : AutowiredConferenceServiceManager<EquipmentService>
    {
        public EquipmentServiceManager(IComponentContext context) : base(context)
        {
        }
    }
}
