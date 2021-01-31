using System;
using System.Collections.Generic;
using Autofac;
using Autofac.Core;
using PaderConference.Core.Services.Media;
using PaderConference.Core.Services.Synchronization;

namespace PaderConference.Infrastructure.Services.Factories
{
    public class MediaServiceManager : AutowiredConferenceServiceManager<MediaService>
    {
        public MediaServiceManager(IComponentContext context) : base(context)
        {
        }

        protected override async IAsyncEnumerable<Parameter> GetParameters(string conferenceId,
            IList<IAsyncDisposable> disposables)
        {
            yield return await ResolveServiceAsync<SynchronizationService>(conferenceId,
                typeof(ISynchronizationManager));
            yield return await ResolveServiceAsync<PermissionsService>(conferenceId);
        }
    }
}