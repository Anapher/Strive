using System;
using System.Collections.Generic;
using Autofac;
using Autofac.Core;
using PaderConference.Core.Domain.Entities;
using PaderConference.Core.Interfaces.Gateways.Repositories;
using PaderConference.Core.Services.ConferenceControl;
using PaderConference.Core.Services.Synchronization;

namespace PaderConference.Infrastructure.Services.Factories
{
    public class ConferenceControlServiceManager : AutowiredConferenceServiceManager<ConferenceControlService>
    {
        private readonly IConferenceRepo _conferenceRepo;

        public ConferenceControlServiceManager(IComponentContext context, IConferenceRepo conferenceRepo) :
            base(context)
        {
            _conferenceRepo = conferenceRepo;
        }

        protected override async IAsyncEnumerable<Parameter> GetParameters(string conferenceId,
            IList<IAsyncDisposable> disposables)
        {
            yield return await ResolveServiceAsync<SynchronizationService>(conferenceId);
            yield return await ResolveServiceAsync<PermissionsService>(conferenceId);

            var conference = await _conferenceRepo.FindById(conferenceId);
            if (conference == null) throw new InvalidOperationException("Conference not found.");

            yield return new TypedParameter(typeof(Conference), conference);
        }
    }
}
