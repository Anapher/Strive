using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using PaderConference.Core.Domain.Entities;
using PaderConference.Core.Extensions;
using PaderConference.Core.Interfaces.Gateways.Repositories;
using PaderConference.Core.Interfaces.Services;

namespace PaderConference.Core.Services.Permissions
{
    /// <summary>
    ///     Provide and synchronize values from the database
    /// </summary>
    public class DatabasePermissionValues : IAsyncDisposable
    {
        private readonly IConferenceRepo _conferenceRepo;
        private readonly IConferenceManager _conferenceManager;
        private readonly string _conferenceId;
        private readonly Func<IEnumerable<Participant>, ValueTask> _refreshParticipants;
        private Func<Task>? _unsubscribeConferenceUpdated;

        public DatabasePermissionValues(IConferenceRepo conferenceRepo, IConferenceManager conferenceManager,
            string conferenceId, Func<IEnumerable<Participant>, ValueTask> refreshParticipants)
        {
            _conferenceRepo = conferenceRepo;
            _conferenceManager = conferenceManager;
            _conferenceId = conferenceId;
            _refreshParticipants = refreshParticipants;
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

        /// <summary>
        ///     A list of the participant ids of all moderators of this conference
        /// </summary>
        public IImmutableList<string> Moderators { get; private set; } = ImmutableList<string>.Empty;

        /// <summary>
        ///     The current conference permissions
        /// </summary>
        public IImmutableDictionary<string, JsonElement>? ConferencePermissions { get; private set; }

        /// <summary>
        ///     The current moderator permissions
        /// </summary>
        public IImmutableDictionary<string, JsonElement>? ModeratorPermissions { get; private set; }

        /// <summary>
        ///     Initialize all properties in this class and subscribe to database events. Please note that
        ///     <see cref="DisposeAsync" /> must be called when this method has executed
        /// </summary>
        public async Task InitializeAsync()
        {
            var conference = await _conferenceRepo.FindById(_conferenceId);
            if (conference == null)
                throw new InvalidOperationException("The conference could not be found in database.");

            Moderators = conference.Moderators;
            ConferencePermissions = ParseDictionary(conference.Permissions);
            ModeratorPermissions = ParseDictionary(conference.ModeratorPermissions);

            _unsubscribeConferenceUpdated =
                await _conferenceRepo.SubscribeConferenceUpdated(_conferenceId, OnConferenceUpdated);
        }

        public async Task OnConferenceUpdated(Conference arg)
        {
            var participants = _conferenceManager.GetParticipants(_conferenceId);
            var updatedParticipants = new HashSet<Participant>();

            // add all users that got their moderator state changed
            var updatedModerators = arg.Moderators.Except(Moderators).Concat(Moderators.Except(arg.Moderators))
                .Distinct();
            updatedParticipants.UnionWith(updatedModerators
                .Select(x => participants.FirstOrDefault(p => p.ParticipantId == x)).WhereNotNull());

            if (!ComparePermissions(arg.ModeratorPermissions, ModeratorPermissions))
            {
                ModeratorPermissions = ParseDictionary(arg.ModeratorPermissions);
                updatedParticipants.UnionWith(arg.Moderators
                    .Select(x => participants.FirstOrDefault(p => p.ParticipantId == x))
                    .WhereNotNull()); // add all current moderators
            }

            if (!ComparePermissions(arg.Permissions, ConferencePermissions))
            {
                ConferencePermissions = ParseDictionary(arg.Permissions);

                // add all participants of the conference
                updatedParticipants.UnionWith(participants);
            }

            Moderators = arg.Moderators;

            if (updatedParticipants.Any())
                await _refreshParticipants(updatedParticipants);
        }

        /// <summary>
        ///     Parse a dictionary, deserialize json values
        /// </summary>
        /// <param name="dictionary">The dictionary with serialized values as string</param>
        /// <returns>Return the dictionary with the values deserialized</returns>
        private static IImmutableDictionary<string, JsonElement>? ParseDictionary(
            IReadOnlyDictionary<string, string>? dictionary)
        {
            return dictionary?.ToImmutableDictionary(x => x.Key, x => JsonSerializer.Deserialize<JsonElement>(x.Value));
        }

        /// <summary>
        ///     Utility method that compares two permission dictionaries
        /// </summary>
        /// <param name="source">The first dictionary</param>
        /// <param name="target">The second dictionary</param>
        /// <returns>Return true if the permission dictionaries are equal (equal keys and values), else return false</returns>
        private static bool ComparePermissions(IReadOnlyDictionary<string, string>? source,
            IReadOnlyDictionary<string, JsonElement>? target)
        {
            if (source == null && target == null) return true;
            if (source == null || target == null) return false;

            return source.EqualItems(target.ToDictionary(x => x.Key, x => JsonSerializer.Serialize(x.Value)));
        }
    }
}
