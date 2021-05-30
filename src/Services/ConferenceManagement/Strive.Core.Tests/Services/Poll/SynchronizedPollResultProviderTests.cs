using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Moq;
using Newtonsoft.Json.Linq;
using Strive.Core.Extensions;
using Strive.Core.Services;
using Strive.Core.Services.Permissions;
using Strive.Core.Services.Poll;
using Strive.Core.Services.Poll.Gateways;
using Strive.Core.Services.Poll.Requests;
using Strive.Core.Services.Poll.Types.MultipleChoice;
using Strive.Tests.Utils;
using Xunit;

namespace Strive.Core.Tests.Services.Poll
{
    public class SynchronizedPollResultProviderTests
    {
        private readonly Participant _participant = new("123", "45");

        private readonly Mock<IPollRepository> _repository = new();
        private readonly Mock<IMediator> _mediator = new();
        private readonly Mock<IParticipantPermissions> _permissions = new();

        private void SetupPermissions(bool hasPermissions)
        {
            var stack = new Dictionary<string, JValue>();
            if (hasPermissions)
            {
                var permission = DefinedPermissions.Poll.CanSeeUnpublishedPollResults.Configure(true);
                stack.Add(permission.Key, permission.Value);
            }

            _permissions.Setup(x => x.FetchForParticipant(_participant)).ReturnsAsync(new CachedPermissionStack(stack));
        }

        private SynchronizedPollResultProvider Create()
        {
            return new(_repository.Object, _mediator.Object, _permissions.Object);
        }

        private Core.Services.Poll.Poll GetPoll(string pollId)
        {
            return new(pollId, new MultipleChoiceInstruction(new[] {"A", "B", "C"}, null), new PollConfig(
                "What is wrong?", false, false), null);
        }

        [Fact]
        public async Task GetAvailableObjects_UserHasPermissionsToSeeUnpublishedResults_ReturnAllPollResults()
        {
            // arrange
            SetupPermissions(true);

            var provider = Create();

            _mediator.HandleRequest<FetchParticipantPollsRequest, IReadOnlyList<Core.Services.Poll.Poll>>(
                new List<Core.Services.Poll.Poll> {GetPoll("1"), GetPoll("2"), GetPoll("3")});

            _repository.Setup(x => x.GetStateOfAllPolls(_participant.ConferenceId)).ReturnsAsync(
                new Dictionary<string, PollState>
                {
                    {"1", new PollState(true, true)},
                    {"2", new PollState(true, false)},
                });

            // act
            var ids = await provider.GetAvailableObjects(_participant);

            // assert
            AssertHelper.AssertScrambledEquals(new[]
            {
                SynchronizedPollResult.SyncObjId("1"), SynchronizedPollResult.SyncObjId("2"),
                SynchronizedPollResult.SyncObjId("3"),
            }.Select(x => x.ToString()), ids.Select(x => x.ToString()));
        }

        [Fact]
        public async Task GetAvailableObjects_UserHasNoPermissions_ReturnPublishedResultsOnly()
        {
            // arrange
            SetupPermissions(false);

            var provider = Create();

            _mediator.HandleRequest<FetchParticipantPollsRequest, IReadOnlyList<Core.Services.Poll.Poll>>(
                new List<Core.Services.Poll.Poll> {GetPoll("1"), GetPoll("2"), GetPoll("3")});

            _repository.Setup(x => x.GetStateOfAllPolls(_participant.ConferenceId)).ReturnsAsync(
                new Dictionary<string, PollState>
                {
                    {"1", new PollState(true, true)},
                    {"2", new PollState(true, false)},
                });

            // act
            var ids = await provider.GetAvailableObjects(_participant);

            // assert
            AssertHelper.AssertScrambledEquals(new[]
            {
                SynchronizedPollResult.SyncObjId("1"),
            }.Select(x => x.ToString()), ids.Select(x => x.ToString()));
        }
    }
}
