using System;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using MediatR;
using Moq;
using Strive.Core.Services;
using Strive.Core.Services.Poll;
using Strive.Core.Services.Poll.Gateways;
using Strive.Core.Services.Poll.Requests;
using Strive.Core.Services.Poll.Types.MultipleChoice;
using Strive.Core.Services.Poll.Types.SingleChoice;
using Strive.Core.Services.Poll.UseCase;
using Xunit;

namespace Strive.Core.Tests.Services.Poll.UseCase
{
    public class SubmitAnswerUseCaseTests
    {
        private const string ConferenceId = "conference";
        private const string PollId = "p1";

        private readonly Participant _participant = new(ConferenceId, "2");

        private readonly Mock<IPollRepository> _repository = new();
        private readonly Mock<IMediator> _mediator = new();

        private SubmitAnswerUseCase Create(IComponentContext context)
        {
            return new(_repository.Object, context, _mediator.Object);
        }

        private Core.Services.Poll.Poll GetPoll(bool isAnswerFinal = false)
        {
            return new(PollId, new MultipleChoiceInstruction(new[] {"A", "B", "C"}, null), new PollConfig(
                "What is wrong?", false, isAnswerFinal), null, DateTimeOffset.MinValue);
        }

        private IComponentContext SetupValidator(bool validatesTo = true)
        {
            var validatorMock = new Mock<IPollAnswerValidator<MultipleChoiceInstruction, MultipleChoiceAnswer>>();
            validatorMock
                .Setup(x => x.Validate(It.IsAny<MultipleChoiceInstruction>(), It.IsAny<MultipleChoiceAnswer>()))
                .Returns(validatesTo ? null : PollError.AnswerValidationFailed("test"));

            var builder = new ContainerBuilder();
            builder.RegisterInstance(validatorMock.Object).AsImplementedInterfaces();
            return builder.Build();
        }

        [Fact]
        public async Task Handle_PollDoesNotExist_Throw()
        {
            // arrange
            var useCase = Create(SetupValidator());

            // act
            var error = await Assert.ThrowsAsync<IdErrorException>(async () =>
                await useCase.Handle(
                    new SubmitAnswerRequest(_participant, PollId, new MultipleChoiceAnswer(new[] {"A"})),
                    CancellationToken.None));

            // assert
            Assert.Equal(ServiceErrorCode.Poll_NotFound.ToString(), error.Error.Code);
        }

        [Fact]
        public async Task Handle_PollStateNull_Throw()
        {
            // arrange
            var useCase = Create(SetupValidator());
            _repository.Setup(x => x.GetPoll(ConferenceId, PollId)).ReturnsAsync(GetPoll());

            // act
            var error = await Assert.ThrowsAsync<IdErrorException>(async () =>
                await useCase.Handle(
                    new SubmitAnswerRequest(_participant, PollId, new MultipleChoiceAnswer(new[] {"A"})),
                    CancellationToken.None));

            // assert
            Assert.Equal(ServiceErrorCode.Poll_Closed.ToString(), error.Error.Code);
        }

        [Fact]
        public async Task Handle_PollNotOpen_Throw()
        {
            // arrange
            var useCase = Create(SetupValidator());
            _repository.Setup(x => x.GetPoll(ConferenceId, PollId)).ReturnsAsync(GetPoll());
            _repository.Setup(x => x.GetPollState(ConferenceId, PollId)).ReturnsAsync(new PollState(false, false));

            // act
            var error = await Assert.ThrowsAsync<IdErrorException>(async () =>
                await useCase.Handle(
                    new SubmitAnswerRequest(_participant, PollId, new MultipleChoiceAnswer(new[] {"A"})),
                    CancellationToken.None));

            // assert
            Assert.Equal(ServiceErrorCode.Poll_Closed.ToString(), error.Error.Code);
        }

        [Fact]
        public async Task Handle_WrongAnswer_Throw()
        {
            // arrange
            var useCase = Create(SetupValidator(false));
            _repository.Setup(x => x.GetPoll(ConferenceId, PollId)).ReturnsAsync(GetPoll());
            _repository.Setup(x => x.GetPollState(ConferenceId, PollId)).ReturnsAsync(new PollState(true, false));

            // act
            var error = await Assert.ThrowsAsync<IdErrorException>(async () =>
                await useCase.Handle(
                    new SubmitAnswerRequest(_participant, PollId, new MultipleChoiceAnswer(new[] {"not existing"})),
                    CancellationToken.None));

            // assert
            Assert.Equal(ServiceErrorCode.AnswerValidationFailed.ToString(), error.Error.Code);
        }

        [Fact]
        public async Task Handle_WrongAnswerType_Throw()
        {
            // arrange
            var useCase = Create(SetupValidator());
            _repository.Setup(x => x.GetPoll(ConferenceId, PollId)).ReturnsAsync(GetPoll());
            _repository.Setup(x => x.GetPollState(ConferenceId, PollId)).ReturnsAsync(new PollState(true, false));

            // act
            await Assert.ThrowsAnyAsync<Exception>(async () =>
                await useCase.Handle(new SubmitAnswerRequest(_participant, PollId, new SingleChoiceAnswer("A")),
                    CancellationToken.None));
        }

        [Fact]
        public async Task Handle_AnswerExistsAndAnswerIsFinal_Throw()
        {
            // arrange
            var useCase = Create(SetupValidator());
            _repository.Setup(x => x.GetPoll(ConferenceId, PollId)).ReturnsAsync(GetPoll(true));
            _repository.Setup(x => x.GetPollState(ConferenceId, PollId)).ReturnsAsync(new PollState(true, false));
            _repository.Setup(x => x.GetPollAnswer(_participant, PollId))
                .ReturnsAsync(new PollAnswerWithKey(new MultipleChoiceAnswer(new[] {"B"}), "123"));

            // act
            var error = await Assert.ThrowsAsync<IdErrorException>(async () =>
                await useCase.Handle(
                    new SubmitAnswerRequest(_participant, PollId, new MultipleChoiceAnswer(new[] {"A"})),
                    CancellationToken.None));

            // assert
            Assert.Equal(ServiceErrorCode.Poll_AnswerCannotBeChanged.ToString(), error.Error.Code);
        }

        [Fact]
        public async Task Handle_Valid_SetAnswer()
        {
            // arrange
            var useCase = Create(SetupValidator());
            _repository.Setup(x => x.GetPoll(ConferenceId, PollId)).ReturnsAsync(GetPoll());
            _repository.Setup(x => x.GetPollState(ConferenceId, PollId)).ReturnsAsync(new PollState(true, false));
            _repository.Setup(x => x.GetPollAnswer(_participant, PollId))
                .ReturnsAsync(new PollAnswerWithKey(new MultipleChoiceAnswer(new[] {"B"}), "123"));

            // act
            await useCase.Handle(new SubmitAnswerRequest(_participant, PollId, new MultipleChoiceAnswer(new[] {"A"})),
                CancellationToken.None);

            // assert
            _repository.Verify(x => x.SetPollAnswer(_participant, PollId, It.IsAny<PollAnswerWithKey>()), Times.Once);
        }

        [Fact]
        public async Task Handle_AnswerNullButFinal_Throw()
        {
            // arrange
            var useCase = Create(SetupValidator());
            _repository.Setup(x => x.GetPoll(ConferenceId, PollId)).ReturnsAsync(GetPoll(true));
            _repository.Setup(x => x.GetPollState(ConferenceId, PollId)).ReturnsAsync(new PollState(true, false));

            // act
            var error = await Assert.ThrowsAsync<IdErrorException>(async () =>
                await useCase.Handle(new SubmitAnswerRequest(_participant, PollId, null), CancellationToken.None));

            // assert
            Assert.Equal(ServiceErrorCode.Poll_AnswerCannotBeChanged.ToString(), error.Error.Code);
        }

        [Fact]
        public async Task Handle_AnswerNull_DeleteAnswer()
        {
            // arrange
            var useCase = Create(SetupValidator());
            _repository.Setup(x => x.GetPoll(ConferenceId, PollId)).ReturnsAsync(GetPoll());
            _repository.Setup(x => x.GetPollState(ConferenceId, PollId)).ReturnsAsync(new PollState(true, false));

            // act
            await useCase.Handle(new SubmitAnswerRequest(_participant, PollId, null), CancellationToken.None);

            // assert
            _repository.Verify(x => x.DeletePollAnswer(_participant, PollId), Times.Once);
        }
    }
}
