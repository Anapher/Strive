using Autofac;
using PaderConference.Core.Services.Media;

namespace PaderConference.Infrastructure.Services.Media
{
    public class MediaServiceManager : AutowiredConferenceServiceManager<MediaService>
    {
        public MediaServiceManager(IComponentContext context) : base(context)
        {
        }
    }
}