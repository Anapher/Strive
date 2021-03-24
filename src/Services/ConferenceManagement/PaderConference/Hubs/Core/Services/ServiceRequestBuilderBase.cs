using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Microsoft.Extensions.Logging;
using PaderConference.Core.Interfaces;
using PaderConference.Extensions;

namespace PaderConference.Hubs.Core.Services
{
    public abstract class ServiceRequestBuilderBase<TResponse> : IServiceRequestBuilder<TResponse>
    {
        private readonly ServiceInvokerContext _context;
        private readonly List<ServiceMiddleware> _middlewares = new();

        protected ServiceRequestBuilderBase(ServiceInvokerContext context)
        {
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

            var requestType = GetRequestType();

            var logger = _context.Context.Resolve<ILogger<ServiceRequestBuilderBase<TResponse>>>();
            using var _ = logger.BeginScope(new Dictionary<string, object>
            {
                {"conferenceId", _context.Participant.ConferenceId},
                {"participantId", _context.Participant.Id},
                {"requestType", requestType.FullName!}
            });

            logger.LogDebug("Send request {requestType}...", requestType);

            try
            {
                return await CreateRequest(token);
            }
            catch (Exception e)
            {
                logger.LogError(e, "An error occurred on sending request {requestType}...", requestType);
                return e.ToError();
            }
        }

        protected abstract Task<SuccessOrError<TResponse>> CreateRequest(CancellationToken token);

        protected abstract Type GetRequestType();
    }
}