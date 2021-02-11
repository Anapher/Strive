using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PaderConference.Core.Interfaces;

namespace PaderConference.Hubs.Services
{
    public class ServiceRequestBuilderSuccessOrError<TResponse> : ServiceRequestBuilderBase<TResponse>
    {
        private readonly IMediator _mediator;
        private readonly Lazy<IRequest<SuccessOrError<TResponse>>> _lazyRequest;

        public ServiceRequestBuilderSuccessOrError(Func<IRequest<SuccessOrError<TResponse>>> requestFactory,
            IMediator mediator, ServiceInvokerContext context) : base(context)
        {
            _lazyRequest = new Lazy<IRequest<SuccessOrError<TResponse>>>(requestFactory);
            _mediator = mediator;
        }

        protected override async Task<SuccessOrError<TResponse>> CreateRequest(CancellationToken token)
        {
            var request = _lazyRequest.Value;
            return await _mediator.Send(request, token);
        }

        protected override Type GetRequestType()
        {
            return _lazyRequest.Value.GetType();
        }
    }
}
