using PaderConference.Core.Domain.Entities;
using Xunit;

namespace PaderConference.Core.Tests.Domain.Entities
{
    public class UserTests
    {
        [Fact]
        public void HasValidRefreshToken_GivenValidToken_ShouldReturnTrue()
        {
            // arrange
            const string refreshToken = "1234";
            var user = new User("", "", "");
            user.AddRefreshToken(refreshToken, "127.0.0.1");

            // act
            var result = user.HasValidRefreshToken(refreshToken);

            Assert.True(result);
        }

        [Fact]
        public void HasValidRefreshToken_GivenExpiredToken_ShouldReturnFalse()
        {
            // arrange
            const string refreshToken = "1234";
            var user = new User("", "", "");
            user.AddRefreshToken(refreshToken, "127.0.0.1", -6); // Provision with token 6 days old

            // act
            var result = user.HasValidRefreshToken(refreshToken);

            Assert.False(result);
        }
    }
}
