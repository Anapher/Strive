using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PaderConference.Core.Services.Chat.Requests;

namespace PaderConference.Core.Services.Chat
{
    public class ParticipantTypingTimer : IParticipantTypingTimer
    {
        private readonly IMediator _mediator;
        private readonly ITaskDelay _taskDelay;
        private readonly object _lock = new();

        private readonly Dictionary<ParticipantInChannel, DateTimeOffset> _timers = new();
        private CancellationTokenSource? _cancellationTokenSource;

        public ParticipantTypingTimer(IMediator mediator, ITaskDelay taskDelay)
        {
            _mediator = mediator;
            _taskDelay = taskDelay;
        }

        public void RemoveParticipantTypingAfter(Participant participant, string channel, TimeSpan timespan)
        {
            var now = DateTimeOffset.UtcNow;
            var timeout = now.Add(timespan);

            lock (_lock)
            {
                var info = new ParticipantInChannel(participant, channel);
                _timers[info] = timeout;

                Reschedule();
            }
        }

        public IEnumerable<string> CancelAllTimers(Participant participant)
        {
            lock (_lock)
            {
                var participantChannels = _timers.Keys.Where(x => x.Participant.Equals(participant)).ToList();
                foreach (var participantChannel in participantChannels)
                {
                    _timers.Remove(participantChannel);
                }

                Reschedule();
                return participantChannels.Select(x => x.Channel).ToList();
            }
        }

        public void CancelTimer(Participant participant, string channel)
        {
            lock (_lock)
            {
                var info = new ParticipantInChannel(participant, channel);
                if (_timers.Remove(info)) Reschedule();
            }
        }

        private async void Reschedule()
        {
            // the lock must be acquired here
            //Debug.Assert(Monitor.IsEntered(_lock));

            ParticipantInChannel nextParticipant;
            DateTimeOffset nextTime;
            CancellationToken token;

            lock (_lock)
            {
                // cancel existing cancellation token and set new token
                if (_cancellationTokenSource != null)
                {
                    _cancellationTokenSource.Cancel();
                    _cancellationTokenSource.Dispose();
                    _cancellationTokenSource = null;
                }

                if (!_timers.Any()) return;

                var cancellationTokenSource = _cancellationTokenSource = new CancellationTokenSource();
                token = cancellationTokenSource.Token;

                (nextParticipant, nextTime) = _timers.OrderBy(x => x.Value).First();
            }

            var timeLeft = nextTime.Subtract(DateTimeOffset.UtcNow);
            if (timeLeft > TimeSpan.Zero)
                try
                {
                    await _taskDelay.Delay(timeLeft, token);
                }
                catch (TaskCanceledException)
                {
                    return;
                }

            bool remove;
            lock (_lock)
            {
                remove = _timers.Remove(nextParticipant);
                Console.WriteLine();
                Debug.Print($"removed {remove} {nextParticipant}");
            }

            if (remove)
                RemoveParticipantTyping(nextParticipant);

            Reschedule();
        }

        private async Task RemoveParticipantTyping(ParticipantInChannel participant)
        {
            await _mediator.Send(new SetParticipantTypingRequest(participant.Participant, participant.Channel, false));
        }

        private record ParticipantInChannel(Participant Participant, string Channel);
    }
}
