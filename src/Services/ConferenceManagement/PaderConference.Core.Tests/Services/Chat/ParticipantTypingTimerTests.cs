using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Moq;
using PaderConference.Core.Services;
using PaderConference.Core.Services.Chat;
using PaderConference.Core.Services.Chat.Requests;
using PaderConference.Tests.Utils;
using Xunit;

namespace PaderConference.Core.Tests.Services.Chat
{
    public class ParticipantTypingTimerTests
    {
        private const string ConferenceId = "conferenceId";
        private const string Channel = "testChatChannel";

        private readonly Participant _testParticipant = new(ConferenceId, "123");
        private readonly Mock<IMediator> _mediator = new();

        private ParticipantTypingTimer Create()
        {
            return new(_mediator.Object);
        }

        [Fact]
        public async Task RemoveParticipantTypingAfter_WaitTimeout_SendRequest()
        {
            // arrange
            var timer = Create();
            var capturedRequest = _mediator.CaptureRequest<SetParticipantTypingRequest, Unit>();

            // act
            timer.RemoveParticipantTypingAfter(_testParticipant, Channel, TimeSpan.FromMilliseconds(50));

            // assert
            capturedRequest.AssertNotReceived();

            await Task.Delay(60);

            capturedRequest.AssertReceived();
        }

        [Fact]
        public async Task RemoveParticipantTypingAfter_AddMultiple_SendAllRequests()
        {
            // arrange
            var timer = Create();

            var receivedRequests = new ConcurrentBag<SetParticipantTypingRequest>();
            _mediator.Setup(x => x.Send(It.IsAny<SetParticipantTypingRequest>(), It.IsAny<CancellationToken>()))
                .Callback((IRequest<Unit> request, CancellationToken _) =>
                    receivedRequests.Add((SetParticipantTypingRequest) request));

            // act
            var expectedRequests = new ConcurrentBag<SetParticipantTypingRequest>();
            var participantIdCounter = 0;

            async Task TestRemoveParticipant()
            {
                var random = new Random();

                for (var i = 0; i < 200; i++)
                {
                    await Task.Delay(random.Next(0, 2));

                    var participantId = Interlocked.Increment(ref participantIdCounter).ToString();
                    var participant = new Participant(ConferenceId, participantId);
                    var delay = TimeSpan.FromMilliseconds(random.Next(0, 31));

                    timer.RemoveParticipantTypingAfter(participant, Channel, delay);

                    expectedRequests.Add(new SetParticipantTypingRequest(participant, Channel, false));
                }
            }

            var tasks = Enumerable.Range(0, 4).Select(x => Task.Run(TestRemoveParticipant)).ToList();
            await Task.WhenAll(tasks);

            await Task.Delay(50);

            // assert
            AssertHelper.AssertScrambledEquals(expectedRequests, receivedRequests);
        }

        [Fact]
        public void CancelTimer_TimerWasNotSet_DoNothing()
        {
            // arrange
            var timer = Create();

            // act
            timer.CancelTimer(_testParticipant, Channel);
        }

        [Fact]
        public async Task CancelTimer_TimerWasSet_NoRequest()
        {
            // arrange
            var timer = Create();
            var capturedRequest = _mediator.CaptureRequest<SetParticipantTypingRequest, Unit>();

            timer.RemoveParticipantTypingAfter(_testParticipant, Channel, TimeSpan.FromMilliseconds(100));

            // act
            timer.CancelTimer(_testParticipant, Channel);

            // assert
            await Task.Delay(120);
            capturedRequest.AssertNotReceived();
        }

        [Fact]
        public async Task CancelTimer_TimerWasSet_Reschedule()
        {
            const string channel2 = "testChannel2";

            // arrange
            var timer = Create();
            var capturedRequest = _mediator.CaptureRequest<SetParticipantTypingRequest, Unit>();

            timer.RemoveParticipantTypingAfter(_testParticipant, Channel, TimeSpan.FromMilliseconds(100));
            timer.RemoveParticipantTypingAfter(_testParticipant, channel2, TimeSpan.FromMilliseconds(101));

            // act
            timer.CancelTimer(_testParticipant, Channel);

            // assert
            await Task.Delay(120);
            capturedRequest.AssertReceived();

            var request = capturedRequest.GetRequest();
            Assert.Equal(channel2, request.Channel);
        }

        [Fact]
        public void CancelAllTimers_TimerWasNotSet_DoNothing()
        {
            // arrange
            var timer = Create();

            // act
            var timers = timer.CancelAllTimers(_testParticipant);

            // assert
            Assert.Empty(timers);
        }

        [Fact]
        public async Task CancelAllTimers_TimerWasSet_CancelAndReturnChannel()
        {
            // arrange
            var timer = Create();
            var capturedRequest = _mediator.CaptureRequest<SetParticipantTypingRequest, Unit>();

            timer.RemoveParticipantTypingAfter(_testParticipant, Channel, TimeSpan.FromMilliseconds(100));

            // act
            var timers = timer.CancelAllTimers(_testParticipant);

            // assert
            Assert.Equal(Channel, Assert.Single(timers));

            await Task.Delay(100);
            capturedRequest.AssertNotReceived();
        }
    }
}
