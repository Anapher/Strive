using System.Threading;
using System.Threading.Tasks;
using Moq;
using PaderConference.Core.Interfaces.Services;
using PaderConference.Core.Services;
using PaderConference.Core.Services.Equipment.Gateways;
using PaderConference.Core.Services.Equipment.Requests;
using PaderConference.Core.Services.Equipment.UseCases;
using Xunit;

namespace PaderConference.Core.Tests.Services.Equipment.UseCases
{
    public class FetchEquipmentTokenUseCaseTests
    {
        private readonly Mock<ITokenFactory> _tokenFactory = new();
        private readonly Mock<IEquipmentTokenRepository> _repo = new();

        private readonly Participant _testParticipant = new("123", "45");

        private FetchEquipmentTokenUseCase Create()
        {
            return new(_tokenFactory.Object, _repo.Object);
        }

        [Fact]
        public async Task Handle_GenerateTokenAndSetInRepository()
        {
            const string testToken = "testToken";

            // arrange
            var useCase = Create();

            _tokenFactory.Setup(x => x.GenerateToken(32)).Returns(testToken);

            // act
            var result = await useCase.Handle(new FetchEquipmentTokenRequest(_testParticipant), CancellationToken.None);

            // assert
            _repo.Verify(x => x.Set(_testParticipant, testToken));
            Assert.Equal(testToken, result);
        }
    }
}
