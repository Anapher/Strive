using Autofac;
using PaderConference.Core.Services.Synchronization;

namespace PaderConference.Infrastructure.Services.Synchronization
{
    public class SynchronizationServiceManager : AutowiredConferenceServiceManager<SynchronizationService>
    {
        public SynchronizationServiceManager(IComponentContext context) : base(context)
        {
        }
    }
}