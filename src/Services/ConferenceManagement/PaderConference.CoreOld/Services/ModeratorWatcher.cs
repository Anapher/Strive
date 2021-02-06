using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using PaderConference.Core.Interfaces.Gateways.Repositories;

namespace PaderConference.Core.Services
{
    /// <summary>
    ///     Watch the moderators of a conference and issue an event if they change.
    /// </summary>
    public class ModeratorWatcher : IAsyncDisposable
    {
        private readonly UseConferenceSelector<IImmutableList<string>> _selector;

        public ModeratorWatcher(string conferenceId, IConferenceRepo conferenceRepo)
        {
            _selector = new UseConferenceSelector<IImmutableList<string>>(conferenceId, conferenceRepo,
                conference => conference.Configuration.Moderators, ImmutableList<string>.Empty);
            _selector.Updated += SelectorOnUpdated;
        }

        private void SelectorOnUpdated(object? sender, ObjectChangedEventArgs<IImmutableList<string>> e)
        {
            var added = e.NewValue.Except(e.OldValue).ToImmutableArray();
            var removed = e.OldValue.Except(e.NewValue).ToImmutableArray();

            if (added.Any() || removed.Any())
                ModeratorsUpdated?.Invoke(this, new ModeratorUpdateInfo(e.NewValue, added, removed));
        }

        /// <summary>
        ///     Called when the moderators have updated for the conference
        /// </summary>
        public event EventHandler<ModeratorUpdateInfo>? ModeratorsUpdated;

        /// <summary>
        ///     A list of the participant ids of all moderators of this conference
        /// </summary>
        public IImmutableList<string> Moderators => _selector.Value;

        /// <summary>
        ///     Initialize all properties in this class and subscribe to database events. Please note that
        ///     <see cref="DisposeAsync" /> must be called when this method has executed
        /// </summary>
        public Task InitializeAsync()
        {
            return _selector.InitializeAsync();
        }

        public ValueTask DisposeAsync()
        {
            return _selector.DisposeAsync();
        }
    }
}
