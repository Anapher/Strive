using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.SignalR.Client;
using PaderConference.Core.Interfaces;
using PaderConference.Core.Services;
using PaderConference.Core.Services.ConferenceControl;
using PaderConference.Hubs;
using PaderConference.IntegrationTests._Helpers;
using Xunit;
using Xunit.Abstractions;

namespace PaderConference.IntegrationTests.Controllers
{
    [Collection(IntegrationTestCollection.Definition)]
    public class ConferenceControlTests : ServiceIntegrationTest
    {
        public ConferenceControlTests(ITestOutputHelper testOutputHelper, MongoDbFixture mongoDb) : base(
            testOutputHelper, mongoDb)
        {
        }

        [Fact]
        public async Task OpenConference_UserNotModerator_PermissionDenied()
        {
            // arrange
            var info = await InitializeConferenceAndConnect();

            // act
            var result = await OpenConference(info);

            // assert
            Assert.False(result.Success);
            AssertErrorCode(ServiceErrorCode.PermissionDenied, result.Error!);

            var conferenceControlObj =
                info.SyncObjects.GetSynchronizedObject<SynchronizedConferenceInfo>(SynchronizedConferenceInfoProvider
                    .SynchronizedObjectId);

            Assert.False(conferenceControlObj.IsOpen);
        }

        [Fact]
        public async Task OpenConference_UserIsModerator_UpdateSynchronizedObject()
        {
            // arrange
            var info = await InitializeConferenceAndConnect(true);

            // act
            var result = await OpenConference(info);

            // assert
            Assert.True(result.Success);

            var conferenceControlObj =
                info.SyncObjects.GetSynchronizedObject<SynchronizedConferenceInfo>(SynchronizedConferenceInfoProvider
                    .SynchronizedObjectId);

            Assert.True(conferenceControlObj.IsOpen);
        }

        [Fact]
        public async Task CloseConference_UserIsModerator_UpdateSynchronizedObject()
        {
            // arrange
            var info = await InitializeConferenceAndConnect(true);
            await OpenConference(info);

            // act
            var result = await info.Connection.InvokeAsync<SuccessOrError<Unit>>(nameof(CoreHub.CloseConference));

            // assert
            Assert.True(result.Success);

            var conferenceControlObj =
                info.SyncObjects.GetSynchronizedObject<SynchronizedConferenceInfo>(SynchronizedConferenceInfoProvider
                    .SynchronizedObjectId);

            Assert.False(conferenceControlObj.IsOpen);
        }
    }
}

