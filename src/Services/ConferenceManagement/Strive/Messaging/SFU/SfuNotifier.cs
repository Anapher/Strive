using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Options;
using Strive.Config;
using Strive.Core.Services;
using Strive.Messaging.SFU.Dto;
using Strive.Messaging.SFU.SendContracts;

namespace Strive.Messaging.SFU
{
    public interface ISfuNotifier
    {
        Task Update(string conferenceId, SfuConferenceInfoUpdate value);

        Task ChangeProducer(string conferenceId, ChangeParticipantProducerDto value);

        Task ParticipantLeft(Participant participant);
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
                new {ConferenceId = conferenceId, Type = "Update", Payload = value},
                context => SetupContext(context, conferenceId));
        }

        public async Task ChangeProducer(string conferenceId, ChangeParticipantProducerDto value)
        {
            await _publishEndpoint.Publish<ChangeParticipantProducer>(
                new {ConferenceId = conferenceId, Type = "ChangeProducer", Payload = value},
                context => SetupContext(context, conferenceId));
        }

        public async Task ParticipantLeft(Participant participant)
        {
            await _publishEndpoint.Publish<ParticipantLeft>(
                new {participant.ConferenceId, Type = "ParticipantLeft", Payload = participant.Id},
                context => SetupContext(context, participant.ConferenceId));
        }

        private void SetupContext(PublishContext context, string conferenceId)
        {
            if (!_options.UseInMemory)
                context.SetRoutingKey(conferenceId);
        }
    }
}
