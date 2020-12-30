using System;
using System.Collections.Generic;
using Autofac;
using Autofac.Core;
using PaderConference.Core.Services.Media;
using PaderConference.Core.Services.Permissions;
using PaderConference.Core.Services.Synchronization;
using PaderConference.Infrastructure.ServiceFactories.Base;

namespace PaderConference.Infrastructure.ServiceFactories
{
    public class MediaServiceManager : AutowiredConferenceServiceManager<MediaService>
    {
        public MediaServiceManager(IComponentContext context) : base(context)
        {
        }

        protected override async IAsyncEnumerable<Parameter> GetParameters(string conferenceId,
            IList<IAsyncDisposable> disposables)
        {
            yield return await ResolveServiceAsync<SynchronizationService>(conferenceId);
            yield return await ResolveServiceAsync<PermissionsService>(conferenceId);
        }
    }
}