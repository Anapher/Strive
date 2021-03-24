using System.Threading.Tasks;
using MediatR;
using PaderConference.Core.Interfaces;

namespace PaderConference.Hubs.Core.Services
{
    public delegate ValueTask<SuccessOrError<Unit>> ServiceMiddleware(ServiceInvokerContext context);
}
