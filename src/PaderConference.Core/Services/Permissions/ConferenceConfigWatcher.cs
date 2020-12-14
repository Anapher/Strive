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
    public class ConferenceConfigWatcher : ModeratorWatcher
    {
        private readonly string _conferenceId;
        private readonly IConferenceRepo _conferenceRepo;
        private readonly IConferenceManager _conferenceManager;
        private readonly Func<IEnumerable<Participant>, ValueTask> _refreshParticipants;

        public ConferenceConfigWatcher(string conferenceId, IConferenceRepo conferenceRepo,
            IConferenceManager conferenceManager, Func<IEnumerable<Participant>, ValueTask> refreshParticipants) : base(
            conferenceId, conferenceRepo)
        {
            _conferenceId = conferenceId;
            _conferenceRepo = conferenceRepo;
            _conferenceManager = conferenceManager;
            _refreshParticipants = refreshParticipants;
        }

        /// <summary>
        ///     The current conference permissions
        /// </summary>
        public IImmutableDictionary<string, JsonElement>? ConferencePermissions { get; private set; }

        /// <summary>
        ///     The current moderator permissions
        /// </summary>
        public IImmutableDictionary<string, JsonElement>? ModeratorPermissions { get; private set; }

        /// <summary>
        ///     Trigger an update, fetch conference from repository and update this object
        /// </summary>
        public async Task TriggerUpdate()
        {
            var conference = await _conferenceRepo.FindById(_conferenceId);
            if (conference != null)
                await OnConferenceUpdated(conference);
        }

        protected override async ValueTask InitializeAsync(Conference conference)
        {
            await base.InitializeAsync(conference);

            ConferencePermissions = ParseDictionary(conference.Permissions);
            ModeratorPermissions = ParseDictionary(conference.ModeratorPermissions);
        }

        protected override async Task OnConferenceUpdated(Conference conference)
        {
            var oldModerators = Moderators;
            await base.OnConferenceUpdated(conference);

            var participants = _conferenceManager.GetParticipants(_conferenceId);
            var updatedParticipants = new HashSet<Participant>();

            // add all users that got their moderator state changed
            var updatedModerators = conference.Moderators.Except(oldModerators)
                .Concat(oldModerators.Except(conference.Moderators)).Distinct();
            updatedParticipants.UnionWith(updatedModerators
                .Select(x => participants.FirstOrDefault(p => p.ParticipantId == x)).WhereNotNull());

            if (!ComparePermissions(conference.ModeratorPermissions, ModeratorPermissions))
            {
                ModeratorPermissions = ParseDictionary(conference.ModeratorPermissions);
                updatedParticipants.UnionWith(conference.Moderators
                    .Select(x => participants.FirstOrDefault(p => p.ParticipantId == x))
                    .WhereNotNull()); // add all current moderators
            }

            if (!ComparePermissions(conference.Permissions, ConferencePermissions))
            {
                ConferencePermissions = ParseDictionary(conference.Permissions);

                // add all participants of the conference
                updatedParticipants.UnionWith(participants);
            }

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
