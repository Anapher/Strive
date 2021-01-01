using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using PaderConference.Core.Domain.Entities;
using PaderConference.Core.Interfaces.Gateways.Repositories;
using PaderConference.Core.Services;
using PaderConference.Infrastructure.Conferencing;
using Serilog;
using Serilog.Extensions.Logging;
using StackExchange.Redis;
using StackExchange.Redis.Extensions.Core.Abstractions;
using Xunit;
using Xunit.Abstractions;

namespace PaderConference.Infrastructure.Tests.Conferencing
{
    public class ConferenceManagerTests
    {
        private readonly ILogger<ConferenceManager> _logger;

        public ConferenceManagerTests(ITestOutputHelper output)
        {
            var logger = new LoggerConfiguration().MinimumLevel.Verbose().WriteTo.TestOutput(output).CreateLogger();
            _logger = new SerilogLoggerFactory(logger).CreateLogger<ConferenceManager>();
        }

        [Fact]
        public async Task OpenConference_ConferenceNotFound_ThrowException()
        {
            // arrange
            var redis = new Mock<IRedisDatabase>();
            var repo = new Mock<IConferenceRepo>();

            var conferenceManager = new ConferenceManager(redis.Object, repo.Object, _logger);

            // act / assert
            await Assert.ThrowsAsync<ConferenceNotFoundException>(async () =>
                await conferenceManager.OpenConference("an id"));
        }

        [Fact]
        public async Task OpenConference_OpenNewConference_SetInDatabaseAndFireEvent()
        {
            // arrange
            var redis = new Mock<IRedisDatabase>();
            var repo = new Mock<IConferenceRepo>();

            repo.Setup(x => x.FindById(It.IsAny<string>())).ReturnsAsync(new Conference("123"));
            redis.Setup(x => x.HashSetAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Conference>(),
                It.IsAny<bool>(), It.IsAny<CommandFlags>())).ReturnsAsync(true);

            var conferenceManager = new ConferenceManager(redis.Object, repo.Object, _logger);
            var eventRaised = false;

            conferenceManager.ConferenceOpened += (_, _) => eventRaised = true;

            // act
            await conferenceManager.OpenConference("an id");

            // assert
            Assert.True(eventRaised);
        }

        [Fact]
        public async Task OpenConference_OpenActiveConference_DontFireEvent()
        {
            // arrange
            var redis = new Mock<IRedisDatabase>();
            var repo = new Mock<IConferenceRepo>();

            repo.Setup(x => x.FindById(It.IsAny<string>())).ReturnsAsync(new Conference("123"));

            var conferenceManager = new ConferenceManager(redis.Object, repo.Object, _logger);
            var eventRaised = false;

            conferenceManager.ConferenceOpened += (_, _) => eventRaised = true;

            // act
            await conferenceManager.OpenConference("an id");

            // assert
            Assert.False(eventRaised);
        }
    }
}
