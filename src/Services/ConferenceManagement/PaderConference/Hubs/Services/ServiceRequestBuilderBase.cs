using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using PaderConference.Core.Interfaces;
using PaderConference.Extensions;

namespace PaderConference.Hubs.Services
{
    public abstract class ServiceRequestBuilderBase<TResponse> : IServiceRequestBuilder<TResponse>
    {
        private readonly ServiceInvokerContext _context;
        private readonly List<ServiceMiddleware> _middlewares = new();

        protected ServiceRequestBuilderBase(ServiceInvokerContext context)
        {
            _context = context;
        }

        protected abstract Task<SuccessOrError<TResponse>> CreateRequest(CancellationToken token);

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
                return await CreateRequest(token);
            }
            catch (Exception e)
            {
                return e.ToError();
            }
        }
    }
}
