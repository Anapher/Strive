using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Moq;
using Strive.Core.Services;
using Strive.Core.Services.Poll;
using Strive.Core.Services.Poll.Gateways;
using Strive.Core.Services.Poll.Requests;
using Strive.Core.Services.Poll.Types.SingleChoice;
using Strive.Core.Services.Poll.UseCase;
using Strive.Core.Services.Rooms;
using Strive.Core.Tests._TestHelpers;
using Xunit;

namespace Strive.Core.Tests.Services.Poll.UseCase
{
    public class FetchParticipantPollsUseCaseTests
    {
        private readonly Participant _participant = new("123", "45");

        private readonly Mock<IPollRepository> _repository = new();
        private readonly Mock<IMediator> _mediator = new();

        private FetchParticipantPollsUseCase Create()
        {
            return new(_repository.Object, _mediator.Object);
        }

        private void SetupParticipantRoom(string? roomId)
        {
            var rooms = new Dictionary<string, string>();
            if (roomId != null)
                rooms[_participant.Id] = roomId;

            _mediator.SetupSyncObj(SynchronizedRooms.SyncObjId,
                new SynchronizedRooms(ImmutableList<Room>.Empty, "123", rooms));
        }

        private void SetupPolls(IEnumerable<Core.Services.Poll.Poll> polls)
        {
            _repository.Setup(x => x.GetPollsOfConference(_participant.ConferenceId)).ReturnsAsync(polls.ToList());
        }

        private static Core.Services.Poll.Poll CreatePoll(string id, string? roomId)
        {
            return new(id, new SingleChoiceInstruction(new[] {"Ja", "Nein"}), new PollConfig("What?", false, false),
                roomId, DateTimeOffset.MinValue);
        }

        [Fact]
        public async Task Handle_ParticipantInNoRoom_ReturnGlobalPolls()
        {
            // arrange
            SetupParticipantRoom(null);
            SetupPolls(new[]
            {
                CreatePoll("1", null),
                CreatePoll("2", "5"),
                CreatePoll("5", null),
            });

            var useCase = Create();

            // act
            var result = await useCase.Handle(new FetchParticipantPollsRequest(_participant), CancellationToken.None);

            // arrange
            Assert.Equal(2, result.Count);
            Assert.Contains(result, x => x.Id == "1");
            Assert.Contains(result, x => x.Id == "5");
        }

        [Fact]
        public async Task Handle_ParticipantInRoom_ReturnGlobalAndRoomPolls()
        {
            // arrange
            SetupParticipantRoom("5");
            SetupPolls(new[]
            {
                CreatePoll("1", null),
                CreatePoll("2", "5"),
                CreatePoll("5", null),
                CreatePoll("2", "6"),
            });

            var useCase = Create();

            // act
            var result = await useCase.Handle(new FetchParticipantPollsRequest(_participant), CancellationToken.None);

            // arrange
            Assert.Equal(3, result.Count);
            Assert.Contains(result, x => x.Id == "1");
            Assert.Contains(result, x => x.Id == "5");
            Assert.Contains(result, x => x.Id == "2");
        }
    }
}
