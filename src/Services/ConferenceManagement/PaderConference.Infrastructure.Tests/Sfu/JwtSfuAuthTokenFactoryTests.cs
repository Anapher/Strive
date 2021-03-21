using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PaderConference.Core.Services;
using PaderConference.Infrastructure.Sfu;
using Xunit;

namespace PaderConference.Infrastructure.Tests.Sfu
{
    public class JwtSfuAuthTokenFactoryTests
    {
        [Fact]
        public async Task GenerateToken_ValidOptions_ReturnToken()
        {
            // arrange
            var options = new SfuJwtOptions
            {
                Audience = "test",
                Issuer = "testIssuer",
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes("53f9f42f6b2845c5a1fdf11a477b8fcd")),
                    SecurityAlgorithms.HmacSha256),
            };

            var factory = new JwtSfuAuthTokenFactory(new OptionsWrapper<SfuJwtOptions>(options));

            // act
            var result = await factory.GenerateToken(new Participant("conferenceId", "123"), "connId");

            // assert
            Assert.NotNull(result);
        }
    }
}
