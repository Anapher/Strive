using System;
using System.Collections.Generic;
using Autofac;
using Autofac.Core;
using PaderConference.Core.Services.Chat;
using PaderConference.Core.Services.Permissions;
using PaderConference.Core.Services.Synchronization;

namespace PaderConference.Infrastructure.Services.Factories
{
    public class ChatServiceManager : AutowiredConferenceServiceManager<ChatService>
    {
        public ChatServiceManager(IComponentContext context) : base(context)
        {
        }

        protected override async IAsyncEnumerable<Parameter> GetParameters(string conferenceId,
            IList<IAsyncDisposable> disposables)
        {
            yield return await ResolveServiceAsync<PermissionsService>(conferenceId);
            yield return await ResolveServiceAsync<SynchronizationService>(conferenceId,
                typeof(ISynchronizationManager));
            yield return await ResolveOptions(conferenceId, disposables, conference => conference.Configuration.Chat);
        }
    }
}