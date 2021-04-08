using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.SignalR.Client;
using Nito.AsyncEx;
using Strive.Core.Interfaces;
using Strive.Core.Services;
using Strive.Core.Services.ConferenceControl;
using Strive.Core.Services.ConferenceManagement;
using Strive.Core.Services.Synchronization;
using Strive.Hubs.Core;
using Strive.Hubs.Core.Dtos;
using Strive.Hubs.Core.Responses;
using Strive.IntegrationTests._Helpers;
using Xunit;
using Xunit.Abstractions;

namespace Strive.IntegrationTests.Services
{
    [Collection(IntegrationTestCollection.Definition)]
    public class ConferenceControlTests : ServiceIntegrationTest
    {
        private static readonly SynchronizedObjectId SyncObjId = SynchronizedConferenceInfo.SyncObjId;

        public ConferenceControlTests(ITestOutputHelper testOutputHelper, MongoDbFixture mongoDb) : base(
            testOutputHelper, mongoDb)
        {
        }

        [Fact]
        public async Task OpenConference_UserNotModerator_PermissionDenied()
        {
            // arrange
            var conference = await CreateConference();
            var connection = await ConnectUserToConference(Moderator, conference);

            // act
            var result = await OpenConference(connection);

            // assert
            Assert.False(result.Success);
            AssertErrorCode(ServiceErrorCode.PermissionDenied, result.Error!);

            var conferenceControlObj =
                connection.SyncObjects.GetSynchronizedObject<SynchronizedConferenceInfo>(SyncObjId);

            Assert.False(conferenceControlObj.IsOpen);
        }

        [Fact]
        public async Task OpenConference_UserIsModerator_UpdateSynchronizedObject()
        {
            // arrange
            var conference = await CreateConference(Moderator);
            var connection = await ConnectUserToConference(Moderator, conference);

            // act
            var result = await OpenConference(connection);

            // assert
            AssertSuccess(result);

            await connection.SyncObjects.AssertSyncObject<SynchronizedConferenceInfo>(SyncObjId,
                value => Assert.True(value.IsOpen));
        }

        [Fact]
        public async Task CloseConference_UserIsModerator_UpdateSynchronizedObject()
        {
            // arrange
            var (connection, _) = await ConnectToOpenedConference();

            // act
            var result = await connection.Hub.InvokeAsync<SuccessOrError<Unit>>(nameof(CoreHub.CloseConference));

            // assert
            AssertSuccess(result);

            await connection.SyncObjects.AssertSyncObject<SynchronizedConferenceInfo>(SyncObjId,
                value => Assert.False(value.IsOpen));
        }

        [Fact]
        public async Task CloseConference_UserIsNotModerator_PermissionDenied()
        {
            // arrange
            var conference = await CreateConference();
            var connection = await ConnectUserToConference(Moderator, conference);

            await OpenConference(connection);

            // act
            var result = await connection.Hub.InvokeAsync<SuccessOrError<Unit>>(nameof(CoreHub.CloseConference));

            // assert
            Assert.False(result.Success);
            AssertErrorCode(ServiceErrorCode.PermissionDenied, result.Error!);
        }

        [Fact]
        public async Task KickParticipant_ParticipantJoined_SendNotificationToParticipant()
        {
            // arrange
            var (connection, conference) = await ConnectToOpenedConference();

            var userToBeKicked = CreateUser();
            var userToBeKickedConnection = await ConnectUserToConference(userToBeKicked, conference);

            var autoResetEvent = new AsyncAutoResetEvent(false);
            userToBeKickedConnection.Hub.On(CoreHubMessages.OnRequestDisconnect,
                (RequestDisconnectDto _) => autoResetEvent.Set());

            // act
            var request = new KickParticipantRequestDto(userToBeKicked.Sub);
            var result =
                await connection.Hub.InvokeAsync<SuccessOrError<Unit>>(nameof(CoreHub.KickParticipant), request);

            // assert
            AssertSuccess(result);

            await autoResetEvent.WaitTimeoutAsync();
        }

        [Fact]
        public async Task PatchConference_ChangeName_UpdateSynchronizedConferenceInfo()
        {
            const string newName = "Hello World";

            // arrange
            var conference = await CreateConference();
            var connection = await ConnectUserToConference(Moderator, conference);

            // act
            var client = Factory.CreateClient();
            connection.User.SetupHttpClient(client);

            var patch = new JsonPatchDocument<ConferenceData>();
            patch.Add(x => x.Configuration.Name, newName);

            var response = await client.PatchAsync($"/v1/conference/{conference.ConferenceId}",
                JsonNetContent.Create(patch));
            response.EnsureSuccessStatusCode();

            // assert
            await connection.SyncObjects.AssertSyncObject<SynchronizedConferenceInfo>(SyncObjId,
                value => Assert.Equal(newName, value.Name));
        }
    }
}
