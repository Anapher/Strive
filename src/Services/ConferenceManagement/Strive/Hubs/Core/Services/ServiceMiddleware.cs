using System.Threading.Tasks;
using MediatR;
using Strive.Core.Interfaces;

namespace Strive.Hubs.Core.Services
{
    public delegate ValueTask<SuccessOrError<Unit>> ServiceMiddleware(ServiceInvokerContext context);
}
