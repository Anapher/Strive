using Strive.Infrastructure.KeyValue.Redis.Scripts;
using Xunit;

namespace Strive.Infrastructure.Tests.KeyValue.Scripts
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
