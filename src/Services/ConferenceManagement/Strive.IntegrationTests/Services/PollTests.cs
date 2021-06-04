using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.SignalR.Client;
using Strive.Core.Errors;
using Strive.Core.Interfaces;
using Strive.Core.Services;
using Strive.Core.Services.Poll;
using Strive.Core.Services.Poll.Types;
using Strive.Core.Services.Poll.Types.SingleChoice;
using Strive.Core.Services.Rooms;
using Strive.Core.Services.Scenes;
using Strive.Core.Services.Scenes.Scenes;
using Strive.Core.Services.Synchronization;
using Strive.Hubs.Core;
using Strive.Hubs.Core.Dtos;
using Strive.IntegrationTests._Helpers;
using Xunit;
using Xunit.Abstractions;

namespace Strive.IntegrationTests.Services
{
    [Collection(IntegrationTestCollection.Definition)]
    public class PollTests : ServiceIntegrationTest
    {
        public PollTests(ITestOutputHelper testOutputHelper, MongoDbFixture mongoDb) : base(testOutputHelper, mongoDb)
        {
        }

        private async Task<string> CreatePollReturnId(UserConnection connection, CreatePollDto dto)
        {
            var result = await connection.Hub.InvokeAsync<SuccessOrError<string>>(nameof(CoreHub.CreatePoll), dto);
            AssertSuccess(result);

            return result.Response!;
        }

        [Fact]
        public async Task CreatePoll_Global_ReceiveSyncPollObj()
        {
            // arrange
            var (connection, _) = await ConnectToOpenedConference();

            // act
            var instruction = new SingleChoiceInstruction(new[] {"A", "B"});
            var createDto = new CreatePollDto(instruction, new PollConfig("A or B?", true, false), PollState.Default,
                null);

            var pollId = await CreatePollReturnId(connection, createDto);

            await connection.SyncObjects.AssertSyncObject<SynchronizedPoll>(SynchronizedPoll.SyncObjId(pollId), poll =>
            {
                Assert.Equal(pollId, poll.Id);
                Assert.Equal(createDto.Config, poll.Config);
                Assert.Equal(createDto.InitialState, poll.State);

                var actualInstruction = Assert.IsType<SingleChoiceInstruction>(poll.Instruction);
                Assert.Equal(instruction.Options, actualInstruction.Options);
            });
        }

        [Fact]
        public async Task CreatePoll_Moderator_ReceiveEmptyResults()
        {
            // arrange
            var (connection, _) = await ConnectToOpenedConference();

            // act
            var createDto = new CreatePollDto(new SingleChoiceInstruction(new[] {"A", "B"}),
                new PollConfig("A or B?", true, false), PollState.Default, null);

            var pollId = await CreatePollReturnId(connection, createDto);

            // assert
            await connection.SyncObjects.AssertSyncObject<SynchronizedPollResult>(
                SynchronizedPollResult.SyncObjId(pollId), results =>
                {
                    Assert.Equal(pollId, results.PollId);
                    Assert.Equal(0, results.ParticipantsAnswered);

                    var pollResults = Assert.IsType<SelectionPollResults>(results.Results);
                    Assert.DoesNotContain(pollResults.Options, x => x.Value.Any());
                });
        }

        [Fact]
        public async Task SubmitAnswer_PollIsAnonymous_PollResultsHasNoTranslationTable()
        {
            // arrange
            var (connection, _) = await ConnectToOpenedConference();

            var createDto = new CreatePollDto(new SingleChoiceInstruction(new[] {"A", "B"}),
                new PollConfig("A or B?", true, false), new PollState(true, false), null);

            var pollId = await CreatePollReturnId(connection, createDto);

            // act
            AssertSuccess(await connection.Hub.InvokeAsync<SuccessOrError<Unit>>(nameof(CoreHub.SubmitPollAnswer),
                new SubmitPollAnswerDto(pollId, new SingleChoiceAnswer("A"))));

            // assert
            await connection.SyncObjects.AssertSyncObject<SynchronizedPollResult>(
                SynchronizedPollResult.SyncObjId(pollId), results =>
                {
                    Assert.Equal(1, results.ParticipantsAnswered);

                    var pollResults = Assert.IsType<SelectionPollResults>(results.Results);
                    Assert.Equal(1, pollResults.Options["A"].Count);
                    Assert.Null(results.TokenIdToParticipant);
                });
        }

        [Fact]
        public async Task SubmitAnswer_AnswerAlreadySubmitted_UpdateAnswer()
        {
            // arrange
            var (connection, _) = await ConnectToOpenedConference();

            var createDto = new CreatePollDto(new SingleChoiceInstruction(new[] {"A", "B"}),
                new PollConfig("A or B?", true, false), new PollState(true, false), null);

            var pollId = await CreatePollReturnId(connection, createDto);

            AssertSuccess(await connection.Hub.InvokeAsync<SuccessOrError<Unit>>(nameof(CoreHub.SubmitPollAnswer),
                new SubmitPollAnswerDto(pollId, new SingleChoiceAnswer("A"))));

            // act
            AssertSuccess(await connection.Hub.InvokeAsync<SuccessOrError<Unit>>(nameof(CoreHub.SubmitPollAnswer),
                new SubmitPollAnswerDto(pollId, new SingleChoiceAnswer("B"))));

            // assert
            await connection.SyncObjects.AssertSyncObject<SynchronizedPollResult>(
                SynchronizedPollResult.SyncObjId(pollId), results =>
                {
                    var pollResults = Assert.IsType<SelectionPollResults>(results.Results);
                    Assert.Empty(pollResults.Options["A"]);
                    Assert.Single(pollResults.Options["B"]);
                });
        }

        [Fact]
        public async Task SubmitAnswer_PollIsNotAnonymous_PollResultsContainsTranslationTable()
        {
            // arrange
            var (connection, _) = await ConnectToOpenedConference();

            var createDto = new CreatePollDto(new SingleChoiceInstruction(new[] {"A", "B"}),
                new PollConfig("A or B?", false, false), new PollState(true, false), null);

            var pollId = await CreatePollReturnId(connection, createDto);

            // act
            AssertSuccess(await connection.Hub.InvokeAsync<SuccessOrError<Unit>>(nameof(CoreHub.SubmitPollAnswer),
                new SubmitPollAnswerDto(pollId, new SingleChoiceAnswer("A"))));

            // assert
            await connection.SyncObjects.AssertSyncObject<SynchronizedPollResult>(
                SynchronizedPollResult.SyncObjId(pollId), results =>
                {
                    var pollResults = Assert.IsType<SelectionPollResults>(results.Results);
                    var participantToken = Assert.Single(pollResults.Options["A"]);

                    Assert.NotNull(results.TokenIdToParticipant);
                    Assert.Equal(connection.User.Sub, results.TokenIdToParticipant![participantToken]);
                });
        }

        [Fact]
        public async Task SubmitAnswer_PollIsOpen_SynchronizeAnswer()
        {
            // arrange
            var (connection, _) = await ConnectToOpenedConference();

            var createDto = new CreatePollDto(new SingleChoiceInstruction(new[] {"A", "B"}),
                new PollConfig("A or B?", true, false), new PollState(true, false), null);

            var pollId = await CreatePollReturnId(connection, createDto);

            // act
            var answer = new SingleChoiceAnswer("A");
            AssertSuccess(await connection.Hub.InvokeAsync<SuccessOrError<Unit>>(nameof(CoreHub.SubmitPollAnswer),
                new SubmitPollAnswerDto(pollId, answer)));

            // assert
            await connection.SyncObjects.AssertSyncObject<SynchronizedPollAnswers>(
                SynchronizedPollAnswers.SyncObjId(connection.User.Sub), results =>
                {
                    var actualAnswer = Assert.Single(results.Answers);
                    Assert.Equal(pollId, actualAnswer.Key);
                    Assert.Equal(answer, actualAnswer.Value.Answer);
                });
        }

        [Fact]
        public async Task DeleteAnswer_AnswerDoesNotExist_DoNothing()
        {
            // arrange
            var (connection, _) = await ConnectToOpenedConference();

            var createDto = new CreatePollDto(new SingleChoiceInstruction(new[] {"A", "B"}),
                new PollConfig("A or B?", true, false), new PollState(true, false), null);

            var pollId = await CreatePollReturnId(connection, createDto);

            // act
            AssertSuccess(await connection.Hub.InvokeAsync<SuccessOrError<Unit>>(nameof(CoreHub.DeletePollAnswer),
                new DeletePollAnswerDto(pollId)));
        }

        [Fact]
        public async Task DeleteAnswer_AnswerDoesExist_RemoveAnswer()
        {
            // arrange
            var (connection, _) = await ConnectToOpenedConference();

            var createDto = new CreatePollDto(new SingleChoiceInstruction(new[] {"A", "B"}),
                new PollConfig("A or B?", true, false), new PollState(true, false), null);

            var pollId = await CreatePollReturnId(connection, createDto);

            var answer = new SingleChoiceAnswer("A");
            AssertSuccess(await connection.Hub.InvokeAsync<SuccessOrError<Unit>>(nameof(CoreHub.SubmitPollAnswer),
                new SubmitPollAnswerDto(pollId, answer)));

            await connection.SyncObjects.AssertSyncObject<SynchronizedPollAnswers>(
                SynchronizedPollAnswers.SyncObjId(connection.User.Sub), results => { Assert.Single(results.Answers); });

            // act
            AssertSuccess(await connection.Hub.InvokeAsync<SuccessOrError<Unit>>(nameof(CoreHub.DeletePollAnswer),
                new DeletePollAnswerDto(pollId)));

            // assert
            await connection.SyncObjects.AssertSyncObject<SynchronizedPollAnswers>(
                SynchronizedPollAnswers.SyncObjId(connection.User.Sub), results => { Assert.Empty(results.Answers); });
        }

        [Fact]
        public async Task SubmitAnswer_PollIsClosed_ReturnError()
        {
            // arrange
            var (connection, _) = await ConnectToOpenedConference();

            var createDto = new CreatePollDto(new SingleChoiceInstruction(new[] {"A", "B"}),
                new PollConfig("A or B?", true, false), new PollState(false, false), null);

            var pollId = await CreatePollReturnId(connection, createDto);

            // act
            var result = await connection.Hub.InvokeAsync<SuccessOrError<Unit>>(nameof(CoreHub.SubmitPollAnswer),
                new SubmitPollAnswerDto(pollId, new SingleChoiceAnswer("A")));

            AssertFailed(result);
            AssertErrorCode(ServiceErrorCode.Poll_Closed, result.Error!);
        }

        [Fact]
        public async Task SubmitAnswer_PollIdNull_ReturnValidationError()
        {
            // arrange
            var (connection, _) = await ConnectToOpenedConference();

            // act
            var result = await connection.Hub.InvokeAsync<SuccessOrError<Unit>>(nameof(CoreHub.SubmitPollAnswer),
                new SubmitPollAnswerDto(null!, new SingleChoiceAnswer("A")));

            AssertFailed(result);
            Assert.Equal(ErrorCode.FieldValidation.ToString(), result.Error?.Code);
        }

        [Fact]
        public async Task CreatePoll_ResultsPublished_NormalUsersReceiveResults()
        {
            // arrange
            var (connection, conference) = await ConnectToOpenedConference();

            var olaf = CreateUser();
            var olafConnection = await ConnectUserToConference(olaf, conference);

            // act
            var pollId = await CreatePollReturnId(connection,
                new CreatePollDto(new SingleChoiceInstruction(new[] {"A", "B"}), new PollConfig("A or B?", true, false),
                    new PollState(true, true), null));

            // assert
            await olafConnection.SyncObjects.AssertSyncObject<SynchronizedPollResult>(
                SynchronizedPollResult.SyncObjId(pollId),
                results => { Assert.IsType<SelectionPollResults>(results.Results); });
        }

        [Fact]
        public async Task CreatePoll_PublishResults_NormalUsersReceiveResults()
        {
            // arrange
            var (connection, conference) = await ConnectToOpenedConference();

            var olaf = CreateUser();
            var olafConnection = await ConnectUserToConference(olaf, conference);

            var pollId = await CreatePollReturnId(connection,
                new CreatePollDto(new SingleChoiceInstruction(new[] {"A", "B"}), new PollConfig("A or B?", true, false),
                    new PollState(true, false), null));

            await olafConnection.SyncObjects.AssertSyncObject<SynchronizedSubscriptions>(
                SynchronizedSubscriptions.SyncObjId(olaf.Sub),
                subscriptions =>
                {
                    Assert.DoesNotContain(subscriptions.Subscriptions.Keys.Select(SynchronizedObjectId.Parse),
                        x => x.Id == SynchronizedObjectIds.POLL_RESULT);
                });

            // act
            AssertSuccess(await connection.Hub.InvokeAsync<SuccessOrError<Unit>>(nameof(CoreHub.UpdatePollState),
                new UpdatePollStateDto(pollId, new PollState(true, true))));

            // assert
            await olafConnection.SyncObjects.AssertSyncObject<SynchronizedPollResult>(
                SynchronizedPollResult.SyncObjId(pollId),
                results => { Assert.IsType<SelectionPollResults>(results.Results); });
        }

        [Fact]
        public async Task DeletePoll_ResultsPublished_RemoveSyncObjs()
        {
            // arrange
            var (connection, _) = await ConnectToOpenedConference();

            var pollId = await CreatePollReturnId(connection,
                new CreatePollDto(new SingleChoiceInstruction(new[] {"A", "B"}), new PollConfig("A or B?", true, false),
                    new PollState(true, true), null));

            AssertSuccess(await connection.Hub.InvokeAsync<SuccessOrError<Unit>>(nameof(CoreHub.SubmitPollAnswer),
                new SubmitPollAnswerDto(pollId, new SingleChoiceAnswer("A"))));

            // act
            AssertSuccess(await connection.Hub.InvokeAsync<SuccessOrError<Unit>>(nameof(CoreHub.DeletePoll),
                new DeletePollDto(pollId)));


            // assert
            await connection.SyncObjects.AssertSyncObject<SynchronizedSubscriptions>(
                SynchronizedSubscriptions.SyncObjId(connection.User.Sub), subscriptions =>
                {
                    Assert.DoesNotContain(subscriptions.Subscriptions.Keys.Select(SynchronizedObjectId.Parse),
                        x => x.Id == SynchronizedObjectIds.POLL_RESULT);
                    Assert.DoesNotContain(subscriptions.Subscriptions.Keys.Select(SynchronizedObjectId.Parse),
                        x => x.Id == SynchronizedObjectIds.POLL);
                });

            await connection.SyncObjects.AssertSyncObject<SynchronizedPollAnswers>(
                SynchronizedPollAnswers.SyncObjId(connection.User.Sub), syncObj => { Assert.Empty(syncObj.Answers); });
        }

        [Fact]
        public async Task CreatePoll_Global_AddScene()
        {
            // arrange
            var (connection, _) = await ConnectToOpenedConference();

            // act
            var createDto = new CreatePollDto(new SingleChoiceInstruction(new[] {"A", "B"}),
                new PollConfig("A or B?", true, false), PollState.Default, null);

            var pollId = await CreatePollReturnId(connection, createDto);

            // assert
            await connection.SyncObjects.AssertSyncObject<SynchronizedScene>(
                SynchronizedScene.SyncObjId(RoomOptions.DEFAULT_ROOM_ID),
                obj => { Assert.Contains(obj.AvailableScenes, x => x is PollScene scene && scene.PollId == pollId); });
        }

        [Fact]
        public async Task DeletePoll_Global_RemoveScene()
        {
            // arrange
            var (connection, _) = await ConnectToOpenedConference();

            var createDto = new CreatePollDto(new SingleChoiceInstruction(new[] {"A", "B"}),
                new PollConfig("A or B?", true, false), PollState.Default, null);

            var pollId = await CreatePollReturnId(connection, createDto);

            await connection.SyncObjects.AssertSyncObject<SynchronizedScene>(
                SynchronizedScene.SyncObjId(RoomOptions.DEFAULT_ROOM_ID),
                obj => { Assert.Contains(obj.AvailableScenes, x => x is PollScene scene && scene.PollId == pollId); });

            // act
            AssertSuccess(await connection.Hub.InvokeAsync<SuccessOrError<Unit>>(nameof(CoreHub.DeletePoll),
                new DeletePollDto(pollId)));

            // assert
            await connection.SyncObjects.AssertSyncObject<SynchronizedScene>(
                SynchronizedScene.SyncObjId(RoomOptions.DEFAULT_ROOM_ID),
                obj => { Assert.DoesNotContain(obj.AvailableScenes, x => x is PollScene scene); });
        }

        [Fact]
        public async Task CloseConference_PollsOpen_DeleteAllPolls()
        {
            // arrange
            var (connection, _) = await ConnectToOpenedConference();

            await CreatePollReturnId(connection,
                new CreatePollDto(new SingleChoiceInstruction(new[] {"A", "B"}), new PollConfig("A or B?", true, false),
                    PollState.Default, null));

            await CreatePollReturnId(connection,
                new CreatePollDto(new SingleChoiceInstruction(new[] {"A", "B"}), new PollConfig("A or B?", true, false),
                    PollState.Default, RoomOptions.DEFAULT_ROOM_ID));

            await connection.SyncObjects.AssertSyncObject<SynchronizedSubscriptions>(
                SynchronizedSubscriptions.SyncObjId(connection.User.Sub),
                obj =>
                {
                    Assert.Equal(2,
                        obj.Subscriptions.Keys.Select(SynchronizedObjectId.Parse)
                            .Count(x => x.Id == SynchronizedObjectIds.POLL));
                });

            // act
            AssertSuccess(await connection.Hub.InvokeAsync<SuccessOrError<Unit>>(nameof(CoreHub.CloseConference)));

            // assert
            await connection.SyncObjects.AssertSyncObject<SynchronizedSubscriptions>(
                SynchronizedSubscriptions.SyncObjId(connection.User.Sub), obj =>
                {
                    // no polls
                    Assert.Equal(0,
                        obj.Subscriptions.Keys.Select(SynchronizedObjectId.Parse)
                            .Count(x => x.Id == SynchronizedObjectIds.POLL));

                    // no poll results
                    Assert.Equal(0,
                        obj.Subscriptions.Keys.Select(SynchronizedObjectId.Parse)
                            .Count(x => x.Id == SynchronizedObjectIds.POLL_RESULT));
                });

            // no poll answers
            await connection.SyncObjects.AssertSyncObject<SynchronizedPollAnswers>(
                SynchronizedPollAnswers.SyncObjId(connection.User.Sub), results => { Assert.Empty(results.Answers); });
        }
    }
}
