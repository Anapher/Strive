using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Moq;
using Strive.Core.Interfaces;
using Strive.Core.Interfaces.Gateways.Repositories;
using Strive.Core.Services.BreakoutRooms;
using Strive.Core.Services.BreakoutRooms.Internal;
using Strive.Core.Services.BreakoutRooms.Requests;
using Strive.Core.Services.BreakoutRooms.UseCases;
using Strive.Core.Services.Rooms.Requests;
using Strive.Tests.Utils;
using Xunit;

namespace Strive.Core.Tests.Services.BreakoutRooms.UseCases
{
    public class OpenBreakoutRoomsUseCaseTests
    {
        private const string ConferenceId = "conferenceId";

        private readonly Mock<IMediator> _mediator = new();

        private OpenBreakoutRoomsUseCase Create()
        {
            return new(_mediator.Object);
        }

        [Fact]
        public async Task Handle_NoAssignmentsAndNotOpen_ApplyNewState()
        {
            // arrange
            var request = new OpenBreakoutRoomsRequest(5, null, "test", null, ConferenceId);
            var useCase = Create();

            var capturedRequest = _mediator.CaptureRequest<ApplyBreakoutRoomRequest, BreakoutRoomInternalState?>();

            // act
            var result = await useCase.Handle(request, CancellationToken.None);

            // assert
            Assert.True(result.Success);
            capturedRequest.AssertReceived();

            var applyRequest = capturedRequest.GetRequest();
            Assert.Equal(ConferenceId, applyRequest.ConferenceId);
            Assert.True(applyRequest.CreateNew);
            Assert.Null(applyRequest.AcquiredLock);
            Assert.NotNull(applyRequest.State);
        }

        [Fact]
        public async Task Handle_NoAssignmentsAndIsOpen_ReturnError()
        {
            // arrange
            var request = new OpenBreakoutRoomsRequest(5, DateTimeOffset.MinValue, "test", null, ConferenceId);
            var useCase = Create();

            _mediator.Setup(x => x.Send(It.IsAny<ApplyBreakoutRoomRequest>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new ConcurrencyException("uff"));

            // act
            var result = await useCase.Handle(request, CancellationToken.None);

            // assert
            Assert.False(result.Success);
            Assert.Equal(BreakoutRoomError.AlreadyOpen.Code, result.Error!.Code);
        }

        [Fact]
        public async Task Handle_AssignToMoreRoomsThanExist_ReturnError()
        {
            // arrange
            var amount = 2;
            var assignedRooms = new[] {new[] {"1", "2"}, new[] {"3", "4"}, new[] {"5", "6"}};

            // act
            var result = await TestAssignments(amount, assignedRooms);
            Assert.False(result.Success);
            Assert.Equal(BreakoutRoomError.AssigningParticipantsFailed.Code, result.Error!.Code);
        }

        [Fact]
        public async Task Handle_AssignToFirstRoomOnly_SendCorrectSetRoomRequests()
        {
            // arrange
            var amount = 5;
            var assignedRooms = new[] {new[] {"1", "2", "3", "4"}};

            // act
            await TestAssignmentsAssertSuccess(amount, assignedRooms,
                new Dictionary<string, string> {{"1", "0"}, {"2", "0"}, {"3", "0"}, {"4", "0"}});
        }

        [Fact]
        public async Task Handle_AssignToAllRooms_SendCorrectSetRoomRequests()
        {
            // arrange
            var amount = 3;
            var assignedRooms = new[] {new[] {"1", "2"}, new[] {"3", "4"}, new[] {"5"}};

            // act
            await TestAssignmentsAssertSuccess(amount, assignedRooms,
                new Dictionary<string, string>
                {
                    {"1", "0"},
                    {"2", "0"},
                    {"3", "1"},
                    {"4", "1"},
                    {"5", "2"},
                });
        }

        private async Task<SuccessOrError<Unit>> TestAssignments(int amount, string[][] assignedRooms)
        {
            // arrange
            var request = new OpenBreakoutRoomsRequest(amount, null, null, assignedRooms, ConferenceId);
            var useCase = Create();

            var openedRooms = Enumerable.Range(0, amount).Select(x => x.ToString()).ToList();
            var internalState = new BreakoutRoomInternalState(
                new BreakoutRoomsConfig(request.Amount, null, request.Description), openedRooms, null);

            _mediator.Setup(x => x.Send(It.IsAny<ApplyBreakoutRoomRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(internalState);

            return await useCase.Handle(request, CancellationToken.None);
        }

        private async Task TestAssignmentsAssertSuccess(int amount, string[][] assignedRooms,
            Dictionary<string, string> expected)
        {
            var result = await TestAssignments(amount, assignedRooms);
            Assert.True(result.Success);

            foreach (var (participantId, roomId) in expected)
            {
                _mediator.Verify(
                    x => x.Send(
                        It.Is<SetParticipantRoomRequest>(request => request.RoomAssignments.Any(assignment =>
                            assignment.participantId == participantId && assignment.roomId == roomId)),
                        It.IsAny<CancellationToken>()), Times.Once);
            }
        }
    }
}
