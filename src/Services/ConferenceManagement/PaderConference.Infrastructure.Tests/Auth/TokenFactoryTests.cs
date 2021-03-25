using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using PaderConference.Infrastructure.Auth;
using Xunit;

namespace PaderConference.Infrastructure.Tests.Auth
{
    public class TokenFactoryTests
    {
        private readonly TokenFactory _factory = new();

        [Fact]
        public void GenerateToken_DefaultSize_ReturnTokenWithCorrectSize()
        {
            // act
            var token = _factory.GenerateToken(20);

            // assert
            Assert.NotNull(token);
            Assert.Equal(20, token.Length);
        }

        [Fact]
        public async Task GenerateToken_ThreadSafe()
        {
            // arrange
            var existingTokens = new ConcurrentDictionary<string, bool>();

            void GenerateTokens()
            {
                for (var i = 0; i < 100000; i++)
                {
                    var token = _factory.GenerateToken();
                    var result = existingTokens.TryAdd(token, true);
                    Assert.True(result);
                }
            }

            // act
            var tasks = Enumerable.Range(0, 3).Select(_ => Task.Run(GenerateTokens)).ToList();
            await Task.WhenAll(tasks);
        }
    }
}
