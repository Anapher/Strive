using System.Threading.Tasks;
using PaderConference.Core.Interfaces;

namespace PaderConference.Hubs.Core.Services
{
    public interface IServiceRequestBuilder<TResponse>
    {
        IServiceRequestBuilder<TResponse> AddMiddleware(ServiceMiddleware func);

        Task<SuccessOrError<TResponse>> Send();
    }
}
