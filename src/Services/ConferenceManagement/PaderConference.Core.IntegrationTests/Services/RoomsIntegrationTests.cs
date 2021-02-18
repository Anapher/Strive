using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using PaderConference.Core.IntegrationTests.Services.Base;
using PaderConference.Core.Interfaces.Gateways.Repositories;
using PaderConference.Core.Services.ConferenceControl.Gateways;
using PaderConference.Core.Services.Rooms;
using PaderConference.Core.Services.Rooms.Notifications;
using PaderConference.Core.Services.Rooms.Requests;
using PaderConference.Core.Services.Synchronization;
using Xunit;
using Xunit.Abstractions;

namespace PaderConference.Core.IntegrationTests.Services
{
    public class RoomsIntegrationTests : ServiceIntegrationTest
    {
        private const string ConferenceId = "123";

        public RoomsIntegrationTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }

        protected override IEnumerable<Type> FetchServiceTypes()
        {
            return FetchTypesOfNamespace(typeof(Room)).Concat(FetchTypesOfNamespace(typeof(SynchronizedObjectId)));
        }

        [Fact]
        public async Task CreateRoomsRequest_ConferenceNotOpen_ConcurrencyException()
        {
            // act
            var room = new RoomCreationInfo("Test");
            await Assert.ThrowsAsync<ConcurrencyException>(async () =>
            {
                await Mediator.Send(new CreateRoomsRequest(ConferenceId, new[] {room}));
            });

            // assert
            Assert.Empty(Data.Data);
            NotificationCollector.AssertNoMoreNotifications();
        }

        [Fact]
        public async Task CreateRoomsRequest_ConferenceIsOpen_ConcurrencyException()
        {
            // arrange
            var conferenceOpenRepo = Container.Resolve<IOpenConferenceRepository>();
            await conferenceOpenRepo.Create(ConferenceId);

            // act
            var room = new RoomCreationInfo("Test");
            await Mediator.Send(new CreateRoomsRequest(ConferenceId, new[] {room}));

            // assert
            NotificationCollector.AssertSingleNotificationIssued<RoomsCreatedNotification>();
        }
    }
}
