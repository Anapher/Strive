using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using PaderConference.Core.Domain.Entities;
using PaderConference.Core.Interfaces.Gateways.Repositories;

namespace PaderConference.Core.Services
{
    /// <summary>
    ///     Watch the moderators of a conference and issue an event if they change.
    /// </summary>
    public class ModeratorWatcher : IAsyncDisposable
    {
        private readonly string _conferenceId;
        private readonly IConferenceRepo _conferenceRepo;
        private Func<Task>? _unsubscribeConferenceUpdated;

        public ModeratorWatcher(string conferenceId, IConferenceRepo conferenceRepo)
        {
            _conferenceId = conferenceId;
            _conferenceRepo = conferenceRepo;
        }

        /// <summary>
        ///     Called when the moderators have updated for the conference
        /// </summary>
        public event EventHandler<ModeratorUpdateInfo>? ModeratorsUpdated;

        /// <summary>
        ///     A list of the participant ids of all moderators of this conference
        /// </summary>
        public IImmutableList<string> Moderators { get; private set; } = ImmutableList<string>.Empty;

        /// <summary>
        ///     Initialize all properties in this class and subscribe to database events. Please note that
        ///     <see cref="DisposeAsync" /> must be called when this method has executed
        /// </summary>
        public async Task InitializeAsync()
        {
            var conference = await _conferenceRepo.FindById(_conferenceId);
            if (conference == null)
                throw new InvalidOperationException("The conference could not be found in database.");

            await Initialize(conference);
            _unsubscribeConferenceUpdated =
                await _conferenceRepo.SubscribeConferenceUpdated(_conferenceId, OnConferenceUpdated);
        }

        protected virtual ValueTask Initialize(Conference conference)
        {
            Moderators = conference.Moderators;
            return new ValueTask();
        }

        public async ValueTask DisposeAsync()
        {
            var unsubscribe = _unsubscribeConferenceUpdated;
            if (unsubscribe != null)
            {
                await unsubscribe();
                _unsubscribeConferenceUpdated = null;
            }
        }

        protected virtual Task OnConferenceUpdated(Conference arg)
        {
            var added = arg.Moderators.Except(Moderators).ToImmutableArray();
            var removed = Moderators.Except(arg.Moderators).ToImmutableArray();

            Moderators = arg.Moderators;

            if (added.Any() || removed.Any())
                ModeratorsUpdated?.Invoke(this, new ModeratorUpdateInfo(arg.Moderators, added, removed));

            return Task.CompletedTask;
        }
    }
}
