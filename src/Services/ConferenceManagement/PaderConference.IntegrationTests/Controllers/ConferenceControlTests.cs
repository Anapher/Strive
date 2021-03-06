using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.SignalR.Client;
using PaderConference.Core.Interfaces;
using PaderConference.Core.Services;
using PaderConference.Core.Services.ConferenceControl;
using PaderConference.Core.Services.Synchronization;
using PaderConference.Hubs;
using PaderConference.Hubs.Dtos;
using PaderConference.Hubs.Responses;
using PaderConference.IntegrationTests._Helpers;
using Xunit;
using Xunit.Abstractions;

namespace PaderConference.IntegrationTests.Controllers
{
    [Collection(IntegrationTestCollection.Definition)]
    public class ConferenceControlTests : ServiceIntegrationTest
    {
        private static readonly SynchronizedObjectId
            SyncObjId = SynchronizedConferenceInfoProvider.SynchronizedObjectId;

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
            Assert.True(result.Success);

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
            Assert.True(result.Success);

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

            var disconnectRequested = false;
            userToBeKickedConnection.Hub.On(CoreHubMessages.Response.OnRequestDisconnect,
                (RequestDisconnectDto _) => disconnectRequested = true);

            // act
            var request = new KickParticipantRequestDto(userToBeKicked.Sub);
            var result =
                await connection.Hub.InvokeAsync<SuccessOrError<Unit>>(nameof(CoreHub.KickParticipant), request);

            // assert
            Assert.True(result.Success);
            Assert.True(disconnectRequested);
        }
    }
}
