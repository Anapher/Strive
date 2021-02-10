using System;
using MediatR;
using PaderConference.Core.Interfaces;

namespace PaderConference.Hubs.Services
{
    public class ServiceInvoker : IServiceInvoker
    {
        private readonly IMediator _mediator;
        private readonly ServiceInvokerContext _context;

        public ServiceInvoker(IMediator mediator, ServiceInvokerContext context)
        {
            _mediator = mediator;
            _context = context;
        }

        public IServiceRequestBuilder<TResponse> Create<TResponse>(IRequest<TResponse> request)
        {
            return new ServiceRequestBuilder<TResponse>(() => request, _mediator, _context);
        }

        public IServiceRequestBuilder<TResponse> Create<TResponse>(Func<IRequest<TResponse>> requestFactory)
        {
            return new ServiceRequestBuilder<TResponse>(requestFactory, _mediator, _context);
        }

        public IServiceRequestBuilder<TResponse> Create<TResponse>(IRequest<SuccessOrError<TResponse>> request)
        {
            return new ServiceRequestBuilderSuccessOrError<TResponse>(() => request, _mediator, _context);
        }

        public IServiceRequestBuilder<TResponse> Create<TResponse>(
            Func<IRequest<SuccessOrError<TResponse>>> requestFactory)
        {
            return new ServiceRequestBuilderSuccessOrError<TResponse>(requestFactory, _mediator, _context);
        }
    }
}
