using System.Threading;
using System.Threading.Tasks;
using Moq;
using Strive.Core.Interfaces.Services;
using Strive.Core.Services;
using Strive.Core.Services.Equipment.Gateways;
using Strive.Core.Services.Equipment.Requests;
using Strive.Core.Services.Equipment.UseCases;
using Xunit;

namespace Strive.Core.Tests.Services.Equipment.UseCases
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
