using Autofac;
using PaderConference.Core.Services.Media;
using PaderConference.Infrastructure.ServiceFactories.Base;

namespace PaderConference.Infrastructure.ServiceFactories
{
    public class MediaServiceManager : AutowiredConferenceServiceManager<MediaService>
    {
        public MediaServiceManager(IComponentContext context) : base(context)
        {
        }
    }
}