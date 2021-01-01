using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Moq;
using PaderConference.Core.Domain.Entities;
using PaderConference.Core.Interfaces.Gateways.Repositories;
using PaderConference.Core.Interfaces.Services;
using PaderConference.Core.Services.Permissions;
using Xunit;

namespace PaderConference.Core.Tests.Services.Permissions
{
    public class ConferenceConfigWatcherTests
    {
        [Fact]
        public async Task InitializeAsync_ConferenceDoesNotExist_ThrowException()
        {
            // arrange
            var conferenceRepo = new Mock<IConferenceRepo>();
            var conferenceManager = new Mock<IConferenceManager>();
            const string conferenceId = "test";

            conferenceRepo.Setup(x => x.FindById(conferenceId)).ReturnsAsync((Conference?) null);

            var refreshParticipants = new Mock<Func<IEnumerable<Participant>, ValueTask>>();

            var databasePermissionValues = new ConferenceConfigWatcher(conferenceId, conferenceRepo.Object,
                conferenceManager.Object, refreshParticipants.Object);

            // act
            await Assert.ThrowsAnyAsync<Exception>(async () => await databasePermissionValues.InitializeAsync());

            // assert
            refreshParticipants.Verify(x => x(It.IsAny<IEnumerable<Participant>>()), Times.Never);
        }

        [Fact]
        public async Task InitializeAsync_ValidConference_StateShouldBeAppliedToObjectProperties()
        {
            // arrange
            var conferenceRepo = new Mock<IConferenceRepo>();
            var conferenceManager = new Mock<IConferenceManager>();
            const string conferenceId = "test";

            var moderators = new List<string> {"test"};
            var conference = CreateConference(conferenceId, moderators);
            conferenceRepo.Setup(x => x.FindById(conferenceId)).ReturnsAsync(conference);

            var refreshParticipants = new Mock<Func<IEnumerable<Participant>, ValueTask>>();

            var databasePermissionValues = new ConferenceConfigWatcher(conferenceId, conferenceRepo.Object,
                conferenceManager.Object, refreshParticipants.Object);

            // act
            await databasePermissionValues.InitializeAsync();

            // assert
            Assert.Equal(moderators, databasePermissionValues.Moderators);
            Assert.Null(databasePermissionValues.ConferencePermissions);
            Assert.Null(databasePermissionValues.ModeratorPermissions);
        }

        [Fact]
        public async Task InitializeAsync_HasPermissionsAndModerators_StateShouldBeAppliedToObjectProperties()
        {
            // arrange
            var conferenceRepo = new Mock<IConferenceRepo>();
            var conferenceManager = new Mock<IConferenceManager>();
            const string conferenceId = "test";

            var moderators = new List<string> {"test"};
            var conference = CreateConference(conferenceId, moderators);
            conference.Permissions[PermissionType.Conference] =
                new Dictionary<string, JsonElement> {{"test2", JsonSerializer.Deserialize<JsonElement>("32")}};
            conference.Permissions[PermissionType.Moderator] =
                new Dictionary<string, JsonElement> {{"test1", JsonSerializer.Deserialize<JsonElement>("true")}};

            conferenceRepo.Setup(x => x.FindById(conferenceId)).ReturnsAsync(conference);

            var refreshParticipants = new Mock<Func<IEnumerable<Participant>, ValueTask>>();

            var databasePermissionValues = new ConferenceConfigWatcher(conferenceId, conferenceRepo.Object,
                conferenceManager.Object, refreshParticipants.Object);

            // act
            await databasePermissionValues.InitializeAsync();

            // assert
            Assert.Equal(moderators, databasePermissionValues.Moderators);
            Assert.NotEmpty(databasePermissionValues.ConferencePermissions);
            Assert.NotEmpty(databasePermissionValues.ModeratorPermissions);
        }

        [Fact]
        public async Task DisposeAsync_CalledAfterInitializeAsync_DisposeEventHandler()
        {
            // arrange
            var conferenceRepo = new Mock<IConferenceRepo>();
            var conferenceManager = new Mock<IConferenceManager>();
            const string conferenceId = "test";

            var conference = new Conference(conferenceId);
            var unsubscribeCallback = new Mock<IAsyncDisposable>();

            conferenceRepo.Setup(x => x.FindById(conferenceId)).ReturnsAsync(conference);
            conferenceRepo.Setup(x => x.SubscribeConferenceUpdated(conferenceId, It.IsAny<Func<Conference, Task>>()))
                .ReturnsAsync(unsubscribeCallback.Object);

            var refreshParticipants = new Mock<Func<IEnumerable<Participant>, ValueTask>>();

            var watcher = new ConferenceConfigWatcher(conferenceId, conferenceRepo.Object, conferenceManager.Object,
                refreshParticipants.Object);

            // act
            await watcher.InitializeAsync();
            await watcher.DisposeAsync();

            // assert
            conferenceRepo.Verify(x => x.SubscribeConferenceUpdated(conferenceId, It.IsAny<Func<Conference, Task>>()),
                Times.Once);
            unsubscribeCallback.Verify(x => x.DisposeAsync(), Times.Once);
        }

        // moderators before, moderators after, participants after, mod permissions updated, conference permissions updated, expected participants updated
        public static readonly TheoryData<string[], string[], string[], bool, bool, string[]> TestConferenceUpdatedData
            = new()
            {
                // no participants, no changes
                {
                    Array.Empty<string>(), Array.Empty<string>(), Array.Empty<string>(), false, false,
                    Array.Empty<string>()
                },

                // participants, no changes
                {new[] {"mod1"}, new[] {"mod1"}, new[] {"mod1", "mod2"}, false, false, Array.Empty<string>()},

                // remove one moderator
                {new[] {"mod1", "mod2"}, new[] {"mod1"}, new[] {"mod1", "mod2"}, false, false, new[] {"mod2"}},

                // add one moderator
                {new[] {"mod1"}, new[] {"mod1", "mod2"}, new[] {"mod1", "mod2"}, false, false, new[] {"mod2"}},

                // update moderator permissions
                {new[] {"mod1"}, new[] {"mod1"}, new[] {"mod1", "mod2"}, true, false, new[] {"mod1"}},

                // update participant permissions
                {new[] {"mod1"}, new[] {"mod1"}, new[] {"mod1", "mod2"}, false, true, new[] {"mod1", "mod2"}},

                // no participants, update moderator permissions
                {
                    Array.Empty<string>(), Array.Empty<string>(), Array.Empty<string>(), true, false,
                    Array.Empty<string>()
                },

                // no participants, update participant permissions
                {
                    Array.Empty<string>(), Array.Empty<string>(), Array.Empty<string>(), false, true,
                    Array.Empty<string>()
                },
            };

        [Theory]
        [MemberData(nameof(TestConferenceUpdatedData))]
        public async Task
            InitializeAsync_SupplyDifferentUpdatesForModeratorsAndPermissions_ShouldApplyNewValuesToObjectStateAndFireEvent(
                string[] moderatorsBefore, string[] updatedModerators, string[] allParticipants,
                bool moderatorPermissionsUpdated, bool conferencePermissionsUpdated,
                string[] expectedParticipantsRefreshed)
        {
            // arrange
            var conferenceRepo = new Mock<IConferenceRepo>();
            var conferenceManager = new Mock<IConferenceManager>();
            const string conferenceId = "test";

            var conference = CreateConference(conferenceId, moderatorsBefore);
            conference.Permissions[PermissionType.Conference] =
                new Dictionary<string, JsonElement> {{"test2", JsonSerializer.Deserialize<JsonElement>("32")}};
            conference.Permissions[PermissionType.Moderator] =
                new Dictionary<string, JsonElement> {{"test1", JsonSerializer.Deserialize<JsonElement>("true")}};

            Func<Conference, Task>? onUpdateHandler = null;

            conferenceRepo.Setup(x => x.FindById(conferenceId)).ReturnsAsync(conference);
            conferenceRepo.Setup(x => x.SubscribeConferenceUpdated(conferenceId, It.IsAny<Func<Conference, Task>>()))
                .Callback((string _, Func<Conference, Task> handler) => onUpdateHandler = handler);

            var conferenceParticipants = allParticipants
                .Select(x => new Participant(x, null, "role", DateTimeOffset.MinValue)).ToList();
            conferenceManager.Setup(x => x.GetParticipants(conferenceId)).Returns(conferenceParticipants);

            var updatedConference = CreateConference(conferenceId, updatedModerators);

            if (!moderatorPermissionsUpdated)
                updatedConference.Permissions[PermissionType.Moderator] =
                    conference.Permissions[PermissionType.Moderator];
            if (!conferencePermissionsUpdated)
                updatedConference.Permissions[PermissionType.Conference] =
                    conference.Permissions[PermissionType.Conference];

            var refreshParticipants = new Mock<Func<IEnumerable<Participant>, ValueTask>>();

            IEnumerable<Participant>? refreshedParticipants = null;
            refreshParticipants.Setup(x => x(It.IsAny<IEnumerable<Participant>>())).Callback(
                (IEnumerable<Participant> result) => { refreshedParticipants = result; });

            var databasePermissionValues = new ConferenceConfigWatcher(conferenceId, conferenceRepo.Object,
                conferenceManager.Object, refreshParticipants.Object);

            // act
            await databasePermissionValues.InitializeAsync();

            Assert.NotEmpty(databasePermissionValues.ModeratorPermissions);
            Assert.NotEmpty(databasePermissionValues.ConferencePermissions);

            Assert.NotNull(onUpdateHandler);

            await onUpdateHandler.Invoke(updatedConference);

            // assert
            if (!expectedParticipantsRefreshed.Any())
            {
                refreshParticipants.Verify(x => x(It.IsAny<IEnumerable<Participant>>()), Times.Never);
            }
            else
            {
                refreshParticipants.Verify(x => x(It.IsAny<IEnumerable<Participant>>()), Times.Once);
                Assert.Equal(expectedParticipantsRefreshed, refreshedParticipants?.Select(x => x.ParticipantId));
            }

            if (moderatorPermissionsUpdated) Assert.Null(databasePermissionValues.ModeratorPermissions);
            if (conferencePermissionsUpdated) Assert.Null(databasePermissionValues.ConferencePermissions);
        }

        private static Conference CreateConference(string conferenceId, IEnumerable<string> moderators)
        {
            return new(conferenceId)
            {
                Configuration = new ConferenceConfiguration {Moderators = moderators.ToImmutableList()},
            };
        }
    }
}
