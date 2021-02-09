using System;
using MediatR;

namespace PaderConference.Hubs.Services
{
    public interface IServiceInvoker
    {
        IServiceRequestBuilder<TResponse> Create<TResponse>(IRequest<TResponse> request);

        IServiceRequestBuilder<TResponse> Create<TResponse>(Func<IRequest<TResponse>> request);
    }
}
