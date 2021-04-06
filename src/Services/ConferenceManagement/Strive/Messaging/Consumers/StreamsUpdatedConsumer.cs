using System.Threading.Tasks;
using MassTransit;
using MediatR;
using Strive.Core.Services.Media.Requests;
using Strive.Messaging.SFU.ReceiveContracts;

namespace Strive.Messaging.Consumers
{
    public class StreamsUpdatedConsumer : IConsumer<StreamsUpdated>
    {
        private readonly IMediator _mediator;

        public StreamsUpdatedConsumer(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task Consume(ConsumeContext<StreamsUpdated> context)
        {
            var message = context.Message;
            await _mediator.Send(new ApplyMediaStateRequest(message.ConferenceId, message.Streams));
        }
    }
}
