using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Strive.Core.Services;
using Strive.Messaging.SFU.Dto;

namespace Strive.Messaging.SFU
{
    public class CachedSfuNotifier : ISfuNotifier, IAsyncDisposable
    {
        private readonly object _lock = new();
        private readonly SfuNotifier _sfuNotifier;
        private readonly Dictionary<string, SfuConferenceInfoUpdateAggregator> _updateAggregators = new();
        private bool _isDisposed;

        public CachedSfuNotifier(SfuNotifier sfuNotifier)
        {
            _sfuNotifier = sfuNotifier;
        }

        public Task Update(string conferenceId, SfuConferenceInfoUpdate value)
        {
            lock (_lock)
            {
                if (_isDisposed) throw new ObjectDisposedException(nameof(CachedSfuNotifier));

                if (!_updateAggregators.TryGetValue(conferenceId, out var aggregator))
                    _updateAggregators[conferenceId] = aggregator = new SfuConferenceInfoUpdateAggregator();

                aggregator.Append(value);
            }

            return Task.CompletedTask;
        }

        public Task ChangeProducer(string conferenceId, ChangeParticipantProducerDto value)
        {
            return _sfuNotifier.ChangeProducer(conferenceId, value);
        }

        public Task ParticipantLeft(Participant participant)
        {
            return _sfuNotifier.ParticipantLeft(participant);
        }

        public async ValueTask DisposeAsync()
        {
            if (_isDisposed) return;

            lock (_lock)
            {
                if (_isDisposed) return;
                _isDisposed = true;
            }

            foreach (var (conferenceId, aggregator) in _updateAggregators)
            {
                var update = aggregator.GetUpdate();
                if (IsUpdateEmpty(update)) continue;

                await _sfuNotifier.Update(conferenceId, update);
            }
        }

        private static bool IsUpdateEmpty(SfuConferenceInfoUpdate update)
        {
            return !update.RemovedParticipants.Any() && !update.ParticipantToRoom.Any() &&
                   !update.ParticipantPermissions.Any();
        }
    }
}
