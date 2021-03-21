using System.Threading.Tasks;
using MassTransit;
using MediatR;
using PaderConference.Core.Services.Media.Requests;
using PaderConference.Messaging.SFU.ReceiveContracts;

namespace PaderConference.Messaging.Consumers
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
