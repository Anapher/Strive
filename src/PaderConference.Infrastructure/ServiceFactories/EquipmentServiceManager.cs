using Autofac;
using PaderConference.Core.Services.Equipment;
using PaderConference.Infrastructure.ServiceFactories.Base;

namespace PaderConference.Infrastructure.ServiceFactories
{
    public class EquipmentServiceManager : AutowiredConferenceServiceManager<EquipmentService>
    {
        public EquipmentServiceManager(IComponentContext context) : base(context)
        {
        }
    }
}
