using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Moq;
using PaderConference.Core.Services.Media.Dtos;
using PaderConference.Core.Services.Media.Gateways;
using PaderConference.Core.Services.Media.Requests;
using PaderConference.Core.Services.Media.UseCases;
using PaderConference.Core.Services.Synchronization.Requests;
using Xunit;

namespace PaderConference.Core.Tests.Services.Media.UseCases
{
    public class ApplyMediaStateUseCaseTests
    {
        private readonly Mock<IMediaStateRepository> _repo = new();
        private readonly Mock<IMediator> _mediator = new();

        private ApplyMediaStateUseCase Create()
        {
            return new(_repo.Object, _mediator.Object);
        }

        [Fact]
        public async Task Handle_UpdateInRepository()
        {
            const string conferenceId = "123";
            var update = ImmutableDictionary<string, ParticipantStreams>.Empty;

            // arrange
            var useCase = Create();

            // act
            await useCase.Handle(new ApplyMediaStateRequest(conferenceId, update), CancellationToken.None);

            // assert
            _repo.Verify(x => x.Set(conferenceId, update), Times.Once);
        }

        [Fact]
        public async Task Handle_UpdateSynchronizedObject()
        {
            const string conferenceId = "123";
            var update = ImmutableDictionary<string, ParticipantStreams>.Empty;

            // arrange
            var useCase = Create();

            // act
            await useCase.Handle(new ApplyMediaStateRequest(conferenceId, update), CancellationToken.None);

            // assert
            _mediator.Verify(x => x.Send(It.IsAny<UpdateSynchronizedObjectRequest>(), It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}
