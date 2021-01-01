using System.Threading.Tasks;
using Moq;
using PaderConference.Core.Domain.Entities;
using PaderConference.Core.Dto.Services;
using PaderConference.Core.Dto.UseCaseRequests;
using PaderConference.Core.Interfaces.Gateways.Repositories;
using PaderConference.Core.UseCases;
using Xunit;

namespace PaderConference.Core.Tests.UseCases
{
    public class CreateConferenceUseCaseTests
    {
        [Fact]
        public async Task Handle_ValidConference_StoreInRepoAndSucceed()
        {
            // arrange
            var conferenceRepo = new Mock<IConferenceRepo>();
            var useCase = new CreateConferenceUseCase(conferenceRepo.Object);

            var conferenceData = new ConferenceData();

            // act
            var result = await useCase.Handle(new CreateConferenceRequest(conferenceData));

            // assert
            Assert.True(result.Success);
            conferenceRepo.Verify(x => x.Create(It.IsAny<Conference>()), Times.Once);
        }
    }
}
