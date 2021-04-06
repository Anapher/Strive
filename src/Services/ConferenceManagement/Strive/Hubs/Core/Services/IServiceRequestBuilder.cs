using System.Threading.Tasks;
using Strive.Core.Interfaces;

namespace Strive.Hubs.Core.Services
{
    public interface IServiceRequestBuilder<TResponse>
    {
        IServiceRequestBuilder<TResponse> AddMiddleware(ServiceMiddleware func);

        Task<SuccessOrError<TResponse>> Send();
    }
}
