using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PaderConference.Core.Interfaces;

namespace PaderConference.Hubs.Services
{
    public class ServiceRequestBuilder<TResponse> : ServiceRequestBuilderBase<TResponse>
    {
        private readonly IMediator _mediator;
        private readonly Lazy<IRequest<TResponse>> _lazyRequest;

        public ServiceRequestBuilder(Func<IRequest<TResponse>> requestFactory, IMediator mediator,
            ServiceInvokerContext context) : base(context)
        {
            _lazyRequest = new Lazy<IRequest<TResponse>>(requestFactory);
            _mediator = mediator;
        }

        protected override async Task<SuccessOrError<TResponse>> CreateRequest(CancellationToken token)
        {
            var request = _lazyRequest.Value;
            return SuccessOrError<TResponse>.Succeeded(await _mediator.Send(request, token));
        }

        protected override Type GetRequestType()
        {
            return _lazyRequest.Value.GetType();
        }
    }
}
