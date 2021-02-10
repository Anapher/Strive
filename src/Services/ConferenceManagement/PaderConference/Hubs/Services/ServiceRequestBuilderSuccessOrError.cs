using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PaderConference.Core.Interfaces;

namespace PaderConference.Hubs.Services
{
    public class ServiceRequestBuilderSuccessOrError<TResponse> : ServiceRequestBuilderBase<TResponse>
    {
        private readonly Func<IRequest<SuccessOrError<TResponse>>> _requestFactory;
        private readonly IMediator _mediator;

        public ServiceRequestBuilderSuccessOrError(Func<IRequest<SuccessOrError<TResponse>>> requestFactory,
            IMediator mediator, ServiceInvokerContext context) : base(context)
        {
            _requestFactory = requestFactory;
            _mediator = mediator;
        }

        protected override async Task<SuccessOrError<TResponse>> CreateRequest(CancellationToken token)
        {
            var request = _requestFactory();
            return await _mediator.Send(request, token);
        }
    }
}
