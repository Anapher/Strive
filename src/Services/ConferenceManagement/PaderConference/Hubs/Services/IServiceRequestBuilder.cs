using System.Threading.Tasks;
using PaderConference.Core.Interfaces;

namespace PaderConference.Hubs.Services
{
    public interface IServiceRequestBuilder<TResponse>
    {
        IServiceRequestBuilder<TResponse> AddMiddleware(ServiceMiddleware func);

        Task<SuccessOrError<TResponse>> Send();
    }
}
