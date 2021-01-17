using System.Threading.Tasks;
using MediatR;
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
            var mediator = new Mock<IMediator>();

            var useCase = new CreateConferenceUseCase(conferenceRepo.Object, mediator.Object);

            var conferenceData = new ConferenceData();

            // act
            var result = await useCase.Handle(new CreateConferenceRequest(conferenceData, "test"));

            // assert
            Assert.True(result.Success);
            conferenceRepo.Verify(x => x.Create(It.IsAny<Conference>()), Times.Once);
        }
    }
}
