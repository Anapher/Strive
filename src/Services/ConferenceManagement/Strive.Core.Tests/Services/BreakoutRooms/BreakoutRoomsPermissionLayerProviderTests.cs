using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Options;
using Moq;
using Newtonsoft.Json.Linq;
using Strive.Core.Domain.Entities;
using Strive.Core.Services;
using Strive.Core.Services.BreakoutRooms;
using Strive.Core.Services.BreakoutRooms.Gateways;
using Strive.Core.Services.Permissions.Options;
using Strive.Core.Services.Rooms;
using Strive.Core.Tests._TestHelpers;
using Xunit;

namespace Strive.Core.Tests.Services.BreakoutRooms
{
    public class BreakoutRoomsPermissionLayerProviderTests
    {
        private const string ConferenceId = "test";
        private readonly Participant _participant = new(ConferenceId, "1");
        private const string DefaultRoomId = "test";

        private readonly Mock<IMediator> _mediator = new();
        private readonly Mock<IBreakoutRoomRepository> _repository = new();
        private readonly DefaultPermissionOptions _options = new();

        private BreakoutRoomsPermissionLayerProvider Create()
        {
            return new(_mediator.Object, _repository.Object, new OptionsWrapper<DefaultPermissionOptions>(_options));
        }

        private void SetupParticipantIsInRoom(string roomId)
        {
            _mediator.SetupSyncObj(SynchronizedRooms.SyncObjId,
                new SynchronizedRooms(Array.Empty<Room>(), DefaultRoomId,
                    new Dictionary<string, string> {{_participant.Id, roomId}}));
        }

        private void SetupOpenedBreakoutRooms(params string[] breakoutRoomIds)
        {
            _repository.Setup(x => x.Get(ConferenceId)).ReturnsAsync(
                new BreakoutRoomInternalState(new BreakoutRoomsConfig(breakoutRoomIds.Length, null, null),
                    breakoutRoomIds, null));
        }

        [Fact]
        public async Task FetchPermissionsForParticipant_ParticipantNoInAnyRoom_ReturnEmpty()
        {
            // arrange
            var provider = Create();
            _mediator.SetupSyncObj(SynchronizedRooms.SyncObjId,
                new SynchronizedRooms(Array.Empty<Room>(), "test", ImmutableDictionary<string, string>.Empty));

            // act
            var layers = await provider.FetchPermissionsForParticipant(_participant);

            // assert
            Assert.Empty(layers);
        }

        [Fact]
        public async Task FetchPermissionsForParticipant_ParticipantInDefaultRoom_ReturnEmpty()
        {
            // arrange
            var provider = Create();
            SetupParticipantIsInRoom(DefaultRoomId);

            // act
            var layers = await provider.FetchPermissionsForParticipant(_participant);

            // assert
            Assert.Empty(layers);
        }

        [Fact]
        public async Task FetchPermissionsForParticipant_BreakoutRoomsNotOpen_ReturnEmpty()
        {
            // arrange
            var provider = Create();

            SetupParticipantIsInRoom("some room");
            _repository.Setup(x => x.Get(ConferenceId)).ReturnsAsync((BreakoutRoomInternalState?) null);

            // act
            var layers = await provider.FetchPermissionsForParticipant(_participant);

            // assert
            Assert.Empty(layers);
        }

        [Fact]
        public async Task FetchPermissionsForParticipant_NotInBreakoutRoom_ReturnEmpty()
        {
            // arrange
            var provider = Create();

            SetupParticipantIsInRoom("some room");
            SetupOpenedBreakoutRooms("some other room");

            // act
            var layers = await provider.FetchPermissionsForParticipant(_participant);

            // assert
            Assert.Empty(layers);
        }

        [Fact]
        public async Task FetchPermissionsForParticipant_IsInBreakoutRoom_ReturnDefaultAndConferenceLayer()
        {
            const string roomId = "some breakout room";

            // arrange
            var provider = Create();
            SetupParticipantIsInRoom(roomId);
            SetupOpenedBreakoutRooms(roomId);

            var conferenceBreakoutRoomPermissions = new Dictionary<string, JValue>();
            _mediator.SetupConference(new Conference(ConferenceId)
            {
                Permissions = new Dictionary<PermissionType, Dictionary<string, JValue>>
                {
                    {PermissionType.BreakoutRoom, conferenceBreakoutRoomPermissions},
                },
            });

            // act
            var layers = (await provider.FetchPermissionsForParticipant(_participant)).ToList();

            // assert
            Assert.Equal(2, layers.Count);
            Assert.Contains(layers, x => x.Permissions == conferenceBreakoutRoomPermissions);
            Assert.Contains(layers, x => x.Permissions == _options.Default[PermissionType.BreakoutRoom]);
        }
    }
}
