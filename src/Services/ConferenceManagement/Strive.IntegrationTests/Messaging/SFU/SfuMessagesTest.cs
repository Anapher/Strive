using System.Linq;
using System.Net.Http.Json;
using System.Threading.Tasks;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Newtonsoft.Json.Linq;
using Strive.Core.Interfaces;
using Strive.Core.Services.Permissions;
using Strive.Core.Services.Rooms;
using Strive.Hubs.Core;
using Strive.Hubs.Core.Dtos;
using Strive.IntegrationTests._Helpers;
using Strive.IntegrationTests.Messaging.SFU._Helpers;
using Strive.Messaging.SFU.Dto;
using Strive.Messaging.SFU.SendContracts;
using Strive.Tests.Utils;
using Xunit;
using Xunit.Abstractions;

namespace Strive.IntegrationTests.Messaging.SFU
{
    [Collection(IntegrationTestCollection.Definition)]
    public class SfuMessagesTest : ServiceIntegrationTest
    {
        private const string ApiKey = "testApiKey";

        public SfuMessagesTest(ITestOutputHelper testOutputHelper, MongoDbFixture mongoDb) : base(testOutputHelper,
            mongoDb)
        {
        }

        private async Task<SfuConferenceInfo> GetCurrentConferenceInfo(string conferenceId)
        {
            var response = await Client.GetAsync($"/v1/sfu/{conferenceId}?apiKey={ApiKey}");
            response.EnsureSuccessStatusCode();

            var info = await response.Content.ReadFromJsonAsync<SfuConferenceInfo>();
            Assert.NotNull(info);

            return info!;
        }

        private async Task<SfuConferenceInfoEndpoint> InitializeSfuConnection(string conferenceId)
        {
            var state = await GetCurrentConferenceInfo(conferenceId);
            var endpoint = new SfuConferenceInfoEndpoint(state, conferenceId);

            ConnectPublishObserver(endpoint);

            return endpoint;
        }

        private void ConnectPublishObserver(IPublishObserver endpoint)
        {
            var busControl = Factory.Services.GetRequiredService<IBusControl>();
            busControl.ConnectPublishObserver(endpoint);
        }

        [Fact]
        public async Task ConnectUser_ConferenceIsOpen_UpdateParticipantsRoom()
        {
            // arrange
            var conference = await CreateConference(Moderator);
            var endpoint = await InitializeSfuConnection(conference.ConferenceId);

            // act
            var connection = await ConnectUserToConference(Moderator, conference);
            var result = await OpenConference(connection);
            AssertSuccess(result);

            // assert
            await AssertHelper.WaitForAssert(() =>
            {
                var participantMap = Assert.Single(endpoint.State.ParticipantToRoom);
                Assert.Equal(Moderator.Sub, participantMap.Key);

                var permissions = Assert.Single(endpoint.State.ParticipantPermissions);
                Assert.Equal(Moderator.Sub, permissions.Key);
                Assert.True(permissions.Value.Audio);
            });
        }

        [Fact]
        public async Task ConnectUser_ConferenceIsClosed_UpdateParticipantsPermissionButRoomEmpty()
        {
            // arrange
            var conference = await CreateConference(Moderator);
            var endpoint = await InitializeSfuConnection(conference.ConferenceId);

            // act
            await ConnectUserToConference(Moderator, conference);

            // assert
            await AssertHelper.WaitForAssert(() =>
            {
                Assert.Empty(endpoint.State.ParticipantToRoom);

                var permissions = Assert.Single(endpoint.State.ParticipantPermissions);
                Assert.Equal(Moderator.Sub, permissions.Key);
                Assert.True(permissions.Value.Audio);
            });
        }

        [Fact]
        public async Task DisconnectUser_ConferenceIsOpen_RemoveParticipant()
        {
            // arrange
            var conference = await CreateConference(Moderator);
            var endpoint = await InitializeSfuConnection(conference.ConferenceId);

            var connection = await ConnectUserToConference(Moderator, conference);
            var result = await OpenConference(connection);
            AssertSuccess(result);

            Assert.NotEmpty(endpoint.State.ParticipantToRoom);
            Assert.NotEmpty(endpoint.State.ParticipantPermissions);

            // act
            await connection.Hub.DisposeAsync();

            // assert
            await AssertHelper.WaitForAssert(() =>
            {
                Assert.Empty(endpoint.State.ParticipantToRoom);
                Assert.Empty(endpoint.State.ParticipantPermissions);
            });
        }

        [Fact]
        public async Task SwitchRoom_ConferenceIsOpen_SendUpdateMessage()
        {
            // arrange
            var conference = await CreateConference(Moderator);
            var endpoint = await InitializeSfuConnection(conference.ConferenceId);

            var connection = await ConnectUserToConference(Moderator, conference);
            var result = await OpenConference(connection);
            AssertSuccess(result);

            var roomCreation = await connection.Hub.InvokeAsync<SuccessOrError<Room[]>>(nameof(CoreHub.CreateRooms),
                new[] {new RoomCreationInfo("test")});
            AssertSuccess(roomCreation);

            var roomId = roomCreation.Response!.Single().RoomId;

            // act
            var roomSwitchResponse = await connection.Hub.InvokeAsync<SuccessOrError<Unit>>(nameof(CoreHub.SwitchRoom),
                new SwitchRoomDto(roomId));
            AssertSuccess(roomSwitchResponse);

            // assert
            await AssertHelper.WaitForAssert(() =>
            {
                var roomMap = Assert.Single(endpoint.State.ParticipantToRoom);
                Assert.Equal(Moderator.Sub, roomMap.Key);
                Assert.Equal(roomId, roomMap.Value);
            });
        }

        [Fact]
        public async Task SetTemporaryPermission_ConferenceIsOpen_SendUpdateMessage()
        {
            // arrange
            var conference = await CreateConference(Moderator);
            var endpoint = await InitializeSfuConnection(conference.ConferenceId);

            var connection = await ConnectUserToConference(Moderator, conference);
            var result = await OpenConference(connection);
            AssertSuccess(result);

            var pleb = CreateUser();
            await ConnectUserToConference(pleb, conference);

            await AssertHelper.WaitForAssert(() =>
                Assert.Contains(endpoint.State.ParticipantPermissions, x => x.Key == pleb.Sub && !x.Value.Screen));

            // act
            var permissionResponse = await connection.Hub.InvokeAsync<SuccessOrError<Unit>>(
                nameof(CoreHub.SetTemporaryPermission),
                new SetTemporaryPermissionDto(pleb.Sub, DefinedPermissions.Media.CanShareScreen.Key,
                    (JValue) JToken.FromObject(true)));
            AssertSuccess(permissionResponse);

            // assert
            await AssertHelper.WaitForAssert(() =>
            {
                Assert.Equal(2, endpoint.State.ParticipantToRoom.Count);
                Assert.Equal(2, endpoint.State.ParticipantPermissions.Count);
                Assert.Contains(endpoint.State.ParticipantPermissions, x => x.Key == pleb.Sub && x.Value.Screen);
            });
        }

        [Fact]
        public async Task DisconnectUser_AnyState_SendParticipantLeft()
        {
            // arrange
            var conference = await CreateConference(Moderator);
            var publishObserver = new Mock<IPublishObserver>();
            ConnectPublishObserver(publishObserver.Object);

            var connection = await ConnectUserToConference(Moderator, conference);

            // act
            await connection.Hub.DisposeAsync();

            // assert
            await AssertHelper.WaitForAssert(() =>
            {
                publishObserver.Verify(
                    x => x.PostPublish(
                        It.Is<PublishContext<ParticipantLeft>>(x => x.Message.Payload == connection.User.Sub)),
                    Times.Once);
            });
        }
    }
}
