using PaderConference.Infrastructure.Redis.Scripts;
using Xunit;

namespace PaderConference.Infrastructure.Tests.Redis.Scripts
{
    public class RedisScriptLoaderTests
    {
        [Fact]
        public void TestScripsLoaded()
        {
            var script = RedisScriptLoader.Load(RedisScript.JoinedParticipantsRepository_RemoveParticipant);
            Assert.NotNull(script);
        }
    }
}
