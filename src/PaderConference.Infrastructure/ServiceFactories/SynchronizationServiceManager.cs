using Autofac;
using PaderConference.Core.Services.Synchronization;
using PaderConference.Infrastructure.ServiceFactories.Base;

namespace PaderConference.Infrastructure.ServiceFactories
{
    public class SynchronizationServiceManager : AutowiredConferenceServiceManager<SynchronizationService>
    {
        public SynchronizationServiceManager(IComponentContext context) : base(context)
        {
        }
    }
}