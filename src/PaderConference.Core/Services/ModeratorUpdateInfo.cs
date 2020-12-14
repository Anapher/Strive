using System.Collections.Immutable;

namespace PaderConference.Core.Services
{
    public record ModeratorUpdateInfo
    {
        public ModeratorUpdateInfo(IImmutableList<string> all, IImmutableList<string> added,
            IImmutableList<string> removed)
        {
            All = all;
            Added = added;
            Removed = removed;
        }

        /// <summary>
        ///     All moderators after the update
        /// </summary>
        public IImmutableList<string> All { get; init; }

        /// <summary>
        ///     The moderators that were added
        /// </summary>
        public IImmutableList<string> Added { get; init; }

        /// <summary>
        ///     The moderators that were removed
        /// </summary>
        public IImmutableList<string> Removed { get; init; }
    }
}
