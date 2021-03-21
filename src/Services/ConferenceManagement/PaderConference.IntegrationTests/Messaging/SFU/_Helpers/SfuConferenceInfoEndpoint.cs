using System;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using PaderConference.Messaging.SFU.Dto;
using PaderConference.Messaging.SFU.SendContracts;

namespace PaderConference.IntegrationTests.Messaging.SFU._Helpers
{
    public class SfuConferenceInfoEndpoint : IPublishObserver
    {
        private readonly string _conferenceId;

        public SfuConferenceInfoEndpoint(SfuConferenceInfo state, string conferenceId)
        {
            _conferenceId = conferenceId;
            State = state;
        }

        public SfuConferenceInfo State { get; private set; }

        public Task PrePublish<T>(PublishContext<T> context) where T : class
        {
            return Task.CompletedTask;
        }

        public Task PostPublish<T>(PublishContext<T> context) where T : class
        {
            if (context.Message is MediaStateChanged message && message.ConferenceId == _conferenceId)
                State = ApplyUpdate(State, message.Payload);

            return Task.CompletedTask;
        }

        public Task PublishFault<T>(PublishContext<T> context, Exception exception) where T : class
        {
            return Task.CompletedTask;
        }

        private static SfuConferenceInfo ApplyUpdate(SfuConferenceInfo current, SfuConferenceInfoUpdate update)
        {
            var newPermissions = current.ParticipantPermissions.ToDictionary(x => x.Key, x => x.Value);
            var newParticipants = current.ParticipantToRoom.ToDictionary(x => x.Key, x => x.Value);

            foreach (var updatePermission in update.ParticipantPermissions)
            {
                newPermissions[updatePermission.Key] = updatePermission.Value;
            }

            foreach (var (participantId, roomId) in update.ParticipantToRoom)
            {
                newParticipants[participantId] = roomId;
            }

            foreach (var removedParticipantId in update.RemovedParticipants)
            {
                newPermissions.Remove(removedParticipantId);
                newParticipants.Remove(removedParticipantId);
            }

            return new SfuConferenceInfo(newParticipants, newPermissions);
        }
    }
}
