using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Autofac.Core;
using PaderConference.Infrastructure.Extensions;

namespace PaderConference.Infrastructure.Services
{
    public class AutowiredConferenceServiceManager<TService> : ConferenceServiceManager<TService>
        where TService : IConferenceService
    {
        private readonly IComponentContext _context;

        public AutowiredConferenceServiceManager(IComponentContext context)
        {
            _context = context;
        }

        protected override ValueTask<TService> ServiceFactory(string conferenceId)
        {
            var serviceManagers = _context.Resolve<IEnumerable<IConferenceServiceManager>>();

            var serviceParam = new ResolvedParameter(
                (param, context) => typeof(IConferenceService).IsAssignableFrom(param.ParameterType),
                (param, context) =>
                {
                    var manager = serviceManagers.First(x => param.ParameterType.IsAssignableFrom(x.ServiceType));
                    return manager.GetService(conferenceId).Result; // TODO: maybe execute this task async before?
                });

            return _context.Resolve<TService>(new NamedParameter("conferenceId", conferenceId), serviceParam)
                .ToValueTask();
        }
    }
}
