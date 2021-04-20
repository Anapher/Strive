using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Options;
using Strive.Core.Domain.Entities;
using Strive.Core.Services.BreakoutRooms.Gateways;
using Strive.Core.Services.ConferenceManagement.Requests;
using Strive.Core.Services.Permissions;
using Strive.Core.Services.Permissions.Options;
using Strive.Core.Services.Rooms;
using Strive.Core.Services.Synchronization.Extensions;
using Strive.Core.Utilities;

namespace Strive.Core.Services.BreakoutRooms
{
    public class BreakoutRoomsPermissionLayerProvider : IPermissionLayerProvider
    {
        private readonly IMediator _mediator;
        private readonly DefaultPermissionOptions _options;
        private readonly Func<string, ValueTask<BreakoutRoomInternalState?>> _fetchBreakoutRoomInternalState;

        public BreakoutRoomsPermissionLayerProvider(IMediator mediator, IBreakoutRoomRepository breakoutRoomRepository,
            IOptions<DefaultPermissionOptions> options)
        {
            _mediator = mediator;
            _options = options.Value;

            _fetchBreakoutRoomInternalState =
                Memorized.Func<string, BreakoutRoomInternalState?>(breakoutRoomRepository.Get);
        }

        public async ValueTask<IEnumerable<PermissionLayer>> FetchPermissionsForParticipant(Participant participant)
        {
            var synchronizedRooms =
                await _mediator.FetchSynchronizedObject<SynchronizedRooms>(participant.ConferenceId,
                    SynchronizedRooms.SyncObjId);

            if (!synchronizedRooms.Participants.TryGetValue(participant.Id, out var roomId))
                return Enumerable.Empty<PermissionLayer>();

            if (roomId == synchronizedRooms.DefaultRoomId)
                return Enumerable.Empty<PermissionLayer>();

            var breakoutRoomState = await _fetchBreakoutRoomInternalState(participant.ConferenceId);
            if (breakoutRoomState?.OpenedRooms.Contains(roomId) != true)
                return Enumerable.Empty<PermissionLayer>();

            var result = new List<PermissionLayer>();

            if (_options.Default.TryGetValue(PermissionType.BreakoutRoom, out var breakoutRoomPermissions))
                result.Add(CommonPermissionLayers.BreakoutRoomDefault(breakoutRoomPermissions));

            var conference = await _mediator.Send(new FindConferenceByIdRequest(participant.ConferenceId));
            if (conference.Permissions.TryGetValue(PermissionType.BreakoutRoom, out var permissions))
                result.Add(CommonPermissionLayers.BreakoutRoom(permissions));

            return result;
        }
    }
}
