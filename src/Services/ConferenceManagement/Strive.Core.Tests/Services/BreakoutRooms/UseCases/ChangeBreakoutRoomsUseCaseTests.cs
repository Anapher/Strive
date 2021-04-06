using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.JsonPatch;
using Moq;
using Nito.Disposables;
using Strive.Core.Services.BreakoutRooms;
using Strive.Core.Services.BreakoutRooms.Gateways;
using Strive.Core.Services.BreakoutRooms.Internal;
using Strive.Core.Services.BreakoutRooms.Requests;
using Strive.Core.Services.BreakoutRooms.UseCases;
using Strive.Tests.Utils;
using Xunit;

namespace Strive.Core.Tests.Services.BreakoutRooms.UseCases
{
    public class ChangeBreakoutRoomsUseCaseTests
    {
        private const string ConferenceId = "123";

        private readonly Mock<IMediator> _mediator = new();
        private readonly Mock<IBreakoutRoomRepository> _repository = new();

        private ChangeBreakoutRoomsUseCase Create()
        {
            return new(_mediator.Object, _repository.Object);
        }

        [Fact]
        public async Task Handle_BreakoutRoomsNotOpen_ReturnError()
        {
            // arrange
            var patch = new JsonPatchDocument<BreakoutRoomsConfig>();
            patch.Add(x => x.Amount, 5);

            var useCase = Create();
            var request = new ChangeBreakoutRoomsRequest(ConferenceId, patch);

            // act
            var result = await useCase.Handle(request, CancellationToken.None);

            // assert
            Assert.False(result.Success);
            Assert.Equal(BreakoutRoomError.NotOpen.Code, result.Error!.Code);
        }

        [Fact]
        public async Task Handle_BreakoutRoomsOpen_SendApplyNewStateRequest()
        {
            // arrange
            var patch = new JsonPatchDocument<BreakoutRoomsConfig>();
            patch.Add(x => x.Amount, 5);

            var currentState = new BreakoutRoomsConfig(2, null, "hello world");
            _repository.Setup(x => x.Get(ConferenceId))
                .ReturnsAsync(new BreakoutRoomInternalState(currentState, ImmutableList<string>.Empty, null));

            _repository.Setup(x => x.LockBreakoutRooms(ConferenceId)).ReturnsAsync(NoopDisposable.Instance);

            var useCase = Create();
            var request = new ChangeBreakoutRoomsRequest(ConferenceId, patch);

            var capturedRequest = _mediator.CaptureRequest<ApplyBreakoutRoomRequest, BreakoutRoomInternalState?>();

            // act
            var result = await useCase.Handle(request, CancellationToken.None);

            // assert
            Assert.True(result.Success);

            var applyRequest = capturedRequest.GetRequest();
            Assert.Equal(ConferenceId, applyRequest.ConferenceId);
            Assert.Equal("hello world", applyRequest.State?.Description);
            Assert.Equal(5, applyRequest.State?.Amount);
            Assert.NotNull(applyRequest.AcquiredLock);
        }
    }
}
