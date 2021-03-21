using System;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Options;
using PaderConference.Config;
using PaderConference.Messaging.SFU.Dto;
using PaderConference.Messaging.SFU.SendContracts;

namespace PaderConference.Messaging.SFU
{
    public interface ISfuNotifier
    {
        Task Update(string conferenceId, SfuConferenceInfoUpdate value);

        Task ChangeProducer(string conferenceId, ChangeParticipantProducerDto value);
    }

    public class SfuNotifier : ISfuNotifier
    {
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly RabbitMqOptions _options;

        public SfuNotifier(IPublishEndpoint publishEndpoint, IOptions<RabbitMqOptions> options)
        {
            _publishEndpoint = publishEndpoint;
            _options = options.Value;
        }

        public async Task Update(string conferenceId, SfuConferenceInfoUpdate value)
        {
            await _publishEndpoint.Publish<MediaStateChanged>(
                new {ConferenceId = conferenceId, Type = "Update", Payload = value}, context =>
                {
                    if (!_options.UseInMemory)
                        context.SetRoutingKey(conferenceId);
                });
        }

        public Task ChangeProducer(string conferenceId, ChangeParticipantProducerDto value)
        {
            throw new NotImplementedException();
        }
    }
}
