using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using PaderConference.Core.Services;
using PaderConference.Core.Services.Permissions;
using PaderConference.Core.Services.Rooms;
using PaderConference.Core.Services.Synchronization.Requests;
using PaderConference.Messaging.SFU.Dto;
using PaderConference.Messaging.SFU.Utils;

namespace PaderConference.Messaging.SFU
{
    public interface ISfuConferenceInfoProvider
    {
        ValueTask<SfuConferenceInfo> Get(string conferenceId);

        ValueTask<SynchronizedRooms> GetSynchronizedRooms(string conferenceId);

        ValueTask<Dictionary<string, SfuParticipantPermissions>> GetPermissions(string conferenceId,
            IEnumerable<string> participantIds);
    }

    public class SfuConferenceInfoProvider : ISfuConferenceInfoProvider
    {
        private readonly IMediator _mediator;

        public SfuConferenceInfoProvider(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async ValueTask<SfuConferenceInfo> Get(string conferenceId)
        {
            var synchronizedRooms = await GetSynchronizedRooms(conferenceId);
            var participantToRoom = synchronizedRooms.Participants;
            var permissions = await GetPermissions(conferenceId, participantToRoom.Keys);

            return new SfuConferenceInfo(participantToRoom, permissions);
        }

        public async ValueTask<SynchronizedRooms> GetSynchronizedRooms(string conferenceId)
        {
            return (SynchronizedRooms) await _mediator.Send(new FetchSynchronizedObjectRequest(conferenceId,
                SynchronizedRoomsProvider.SynchronizedObjectId));
        }

        public async ValueTask<Dictionary<string, SfuParticipantPermissions>> GetPermissions(string conferenceId,
            IEnumerable<string> participantIds)
        {
            var permissions = new Dictionary<string, SfuParticipantPermissions>();

            foreach (var participantId in participantIds)
            {
                var participantPermissions =
                    await GetSynchronizedPermissions(new Participant(conferenceId, participantId));

                var sfuPermissions = await SfuPermissionUtils.MapToSfuPermissions(participantPermissions.Permissions);
                permissions.Add(participantId, sfuPermissions);
            }

            return permissions;
        }

        public async ValueTask<SynchronizedParticipantPermissions> GetSynchronizedPermissions(Participant participant)
        {
            return (SynchronizedParticipantPermissions) await _mediator.Send(
                new FetchSynchronizedObjectRequest(participant.ConferenceId,
                    SynchronizedParticipantPermissionsProvider.GetObjIdOfParticipant(participant.Id)));
        }
    }
}
