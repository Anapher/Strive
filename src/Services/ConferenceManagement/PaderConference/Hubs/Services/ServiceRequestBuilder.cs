using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using PaderConference.Core.Interfaces;
using PaderConference.Extensions;

namespace PaderConference.Hubs.Services
{
    public class ServiceRequestBuilder<TResponse> : IServiceRequestBuilder<TResponse>
    {
        private readonly Func<IRequest<TResponse>> _requestFactory;
        private readonly IMediator _mediator;
        private readonly ServiceInvokerContext _context;
        private readonly List<ServiceMiddleware> _middlewares = new();

        public ServiceRequestBuilder(Func<IRequest<TResponse>> requestFactory, IMediator mediator,
            ServiceInvokerContext context)
        {
            _requestFactory = requestFactory;
            _mediator = mediator;
            _context = context;
        }

        public IServiceRequestBuilder<TResponse> AddMiddleware(ServiceMiddleware func)
        {
            _middlewares.Add(func);
            return this;
        }

        public async Task<SuccessOrError<TResponse>> Send()
        {
            var token = _context.Hub.Context.ConnectionAborted;

            foreach (var middleware in _middlewares)
            {
                var result = await middleware(_context);
                if (!result.Success) return result.Error;

                token.ThrowIfCancellationRequested();
            }

            try
            {
                var request = _requestFactory();
                return await _mediator.Send(request, token);
            }
            catch (Exception e)
            {
                return e.ToError();
            }
        }
    }
}
