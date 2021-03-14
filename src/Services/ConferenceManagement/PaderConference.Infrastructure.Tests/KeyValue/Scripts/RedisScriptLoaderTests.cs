using PaderConference.Infrastructure.KeyValue.Redis.Scripts;
using Xunit;

namespace PaderConference.Infrastructure.Tests.KeyValue.Scripts
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
