using System.Threading.Tasks;
using Autofac;
using MediatR;
using Strive.Core;
using Strive.Core.Interfaces;
using Strive.Core.Services.ConferenceControl.Gateways;

namespace Strive.Hubs.Core.Services.Middlewares
{
    public static class ServiceInvokerConferenceMiddleware
    {
        public static IServiceRequestBuilder<TResponse> ConferenceMustBeOpen<TResponse>(
            this IServiceRequestBuilder<TResponse> builder)
        {
            return builder.AddMiddleware(ValidateConferenceIsOpen);
        }

        public static async ValueTask<SuccessOrError<Unit>> ValidateConferenceIsOpen(ServiceInvokerContext context)
        {
            var openConferenceRepo = context.Context.Resolve<IOpenConferenceRepository>();
            if (!await openConferenceRepo.IsOpen(context.Participant.ConferenceId))
                return ConferenceError.ConferenceNotOpen;

            return SuccessOrError<Unit>.Succeeded(Unit.Value);
        }
    }
}