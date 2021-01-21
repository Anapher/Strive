using System;
using System.Collections.Generic;
using Autofac;
using Autofac.Core;
using PaderConference.Core.Services.Permissions;
using PaderConference.Core.Services.Rooms;
using PaderConference.Core.Services.Synchronization;

namespace PaderConference.Infrastructure.Services.Factories
{
    public class RoomsServiceManager : AutowiredConferenceServiceManager<RoomsService>
    {
        public RoomsServiceManager(IComponentContext context) : base(context)
        {
        }

        protected override async IAsyncEnumerable<Parameter> GetParameters(string conferenceId,
            IList<IAsyncDisposable> disposables)
        {
            yield return await ResolveServiceAsync<PermissionsService>(conferenceId);
            yield return await ResolveServiceAsync<SynchronizationService>(conferenceId,
                typeof(ISynchronizationManager));
        }
    }
}
