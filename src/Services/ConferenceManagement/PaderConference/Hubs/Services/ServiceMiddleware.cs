using System.Threading.Tasks;
using MediatR;
using PaderConference.Core.Interfaces;

namespace PaderConference.Hubs.Services
{
    public delegate ValueTask<SuccessOrError<Unit>> ServiceMiddleware(ServiceInvokerContext context);
}
