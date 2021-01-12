using System;
using System.Collections.Generic;
using Autofac;
using Autofac.Core;
using PaderConference.Core.Services.ParticipantsList;
using PaderConference.Core.Services.Synchronization;

namespace PaderConference.Infrastructure.Services.Factories
{
    public class ParticipantsListServiceManager : AutowiredConferenceServiceManager<ParticipantsListService>
    {
        public ParticipantsListServiceManager(IComponentContext context) : base(context)
        {
        }

        protected override async IAsyncEnumerable<Parameter> GetParameters(string conferenceId,
            IList<IAsyncDisposable> disposables)
        {
            yield return await ResolveServiceAsync<SynchronizationService>(conferenceId,
                typeof(ISynchronizationManager));
        }
    }
}
