using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PaderConference.Core.IntegrationTests.Services.Base;
using PaderConference.Core.Interfaces.Gateways.Repositories;
using PaderConference.Core.Services.Rooms;
using PaderConference.Core.Services.Rooms.Requests;
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
            return FetchTypesOfNamespace(typeof(Room));
        }

        [Fact]
        public async Task CreateRoomsRequest_ConferenceNotOpen_ConcurrencyException()
        {
            var room = new RoomCreationInfo("Test");
            await Assert.ThrowsAsync<ConcurrencyException>(async () =>
            {
                await Mediator.Send(new CreateRoomsRequest(ConferenceId, new[] {room}));
            });
            Assert.Empty(Data.Data);
        }

        [Fact]
        public async Task CreateRoomsRequest_ConferenceIsOpen_ConcurrencyException()
        {
            var room = new RoomCreationInfo("Test");
            await Assert.ThrowsAsync<ConcurrencyException>(async () =>
            {
                await Mediator.Send(new CreateRoomsRequest(ConferenceId, new[] {room}));
            });

            Assert.Empty(Data.Data);
        }
    }
}
