using System;
using System.Collections.Generic;
using Autofac;
using Autofac.Core;
using PaderConference.Core.Services.BreakoutRoom;
using PaderConference.Core.Services.Permissions;
using PaderConference.Core.Services.Rooms;
using PaderConference.Core.Services.Synchronization;
using PaderConference.Infrastructure.ServiceFactories.Base;

namespace PaderConference.Infrastructure.ServiceFactories
{
    public class BreakoutRoomServiceManager : AutowiredConferenceServiceManager<BreakoutRoomService>
    {
        public BreakoutRoomServiceManager(IComponentContext context) : base(context)
        {
        }

        protected override async IAsyncEnumerable<Parameter> GetParameters(string conferenceId,
            IList<IAsyncDisposable> disposables)
        {
            yield return await ResolveServiceAsync<PermissionsService>(conferenceId);
            yield return await ResolveServiceAsync<SynchronizationService>(conferenceId);
            yield return await ResolveServiceAsync<RoomsService>(conferenceId);
        }
    }
}
