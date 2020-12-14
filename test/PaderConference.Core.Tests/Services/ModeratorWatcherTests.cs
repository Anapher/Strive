using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;
using Moq;
using PaderConference.Core.Domain.Entities;
using PaderConference.Core.Interfaces.Gateways.Repositories;
using PaderConference.Core.Services;
using Xunit;

namespace PaderConference.Core.Tests.Services
{
    public class ModeratorWatcherTests
    {
        private const string ConferenceId = "conferenceid";

        [Fact]
        public async Task TestConferenceDoesNotExist()
        {
            // arrange
            var conferenceRepo = new Mock<IConferenceRepo>();

            var watcher = new ModeratorWatcher(ConferenceId, conferenceRepo.Object);

            // act
            await Assert.ThrowsAnyAsync<Exception>(() => watcher.InitializeAsync());
        }

        [Fact]
        public async Task TestInitialize()
        {
            // arrange
            var moderators = new List<string> {"test"};
            var conference = new Conference(ConferenceId, moderators.ToImmutableList());

            var conferenceRepo = new Mock<IConferenceRepo>();
            conferenceRepo.Setup(x => x.FindById(ConferenceId)).ReturnsAsync(conference);

            var watcher = new ModeratorWatcher(ConferenceId, conferenceRepo.Object);

            // act
            await watcher.InitializeAsync();

            // assert
            Assert.Equal(moderators, watcher.Moderators);
        }

        [Fact]
        public async Task TestInitializeAndDispose()
        {
            // arrange
            var conference = new Conference(ConferenceId, ImmutableList<string>.Empty);

            var unsubscribeCallback = new Mock<Func<Task>>();

            var conferenceRepo = new Mock<IConferenceRepo>();
            conferenceRepo.Setup(x => x.FindById(ConferenceId)).ReturnsAsync(conference);
            conferenceRepo.Setup(x => x.SubscribeConferenceUpdated(ConferenceId, It.IsAny<Func<Conference, Task>>()))
                .ReturnsAsync(unsubscribeCallback.Object);

            var watcher = new ModeratorWatcher(ConferenceId, conferenceRepo.Object);

            // act
            await watcher.InitializeAsync();
            await watcher.DisposeAsync();

            // assert
            unsubscribeCallback.Verify(x => x(), Times.Once);
        }

        [Fact]
        public async Task TestModeratorsAdded()
        {
            // arrange
            var existingMods = new[] {"test1"};
            var conference = new Conference(ConferenceId, existingMods.ToImmutableList());

            Func<Conference, Task>? onUpdateHandler = null;

            var conferenceRepo = new Mock<IConferenceRepo>();
            conferenceRepo.Setup(x => x.FindById(ConferenceId)).ReturnsAsync(conference);
            conferenceRepo.Setup(x => x.SubscribeConferenceUpdated(ConferenceId, It.IsAny<Func<Conference, Task>>()))
                .Callback((string _, Func<Conference, Task> handler) => onUpdateHandler = handler);

            var watcher = new ModeratorWatcher(ConferenceId, conferenceRepo.Object);
            var calls = 0;

            var newMods = new[] {"test1", "mod1"};

            watcher.ModeratorsUpdated += (sender, info) =>
            {
                Assert.Equal(newMods, info.All);
                Assert.Equal(new[] {"mod1"}, info.Added);
                Assert.Empty(info.Removed);
                calls++;
            };

            // act
            await watcher.InitializeAsync();

            await onUpdateHandler(new Conference(ConferenceId, newMods.ToImmutableList()));

            // assert
            Assert.Equal(1, calls);
        }

        [Fact]
        public async Task TestModeratorsRemoved()
        {
            // arrange
            var existingMods = new[] {"test1"};
            var conference = new Conference(ConferenceId, existingMods.ToImmutableList());

            Func<Conference, Task>? onUpdateHandler = null;

            var conferenceRepo = new Mock<IConferenceRepo>();
            conferenceRepo.Setup(x => x.FindById(ConferenceId)).ReturnsAsync(conference);
            conferenceRepo.Setup(x => x.SubscribeConferenceUpdated(ConferenceId, It.IsAny<Func<Conference, Task>>()))
                .Callback((string _, Func<Conference, Task> handler) => onUpdateHandler = handler);

            var watcher = new ModeratorWatcher(ConferenceId, conferenceRepo.Object);
            var calls = 0;

            var newMods = new string[0];

            watcher.ModeratorsUpdated += (sender, info) =>
            {
                Assert.Equal(newMods, info.All);
                Assert.Equal(new[] {"test1"}, info.Removed);
                Assert.Empty(info.Added);
                calls++;
            };

            // act
            await watcher.InitializeAsync();

            await onUpdateHandler(new Conference(ConferenceId, newMods.ToImmutableList()));

            // assert
            Assert.Equal(1, calls);
        }
    }
}
