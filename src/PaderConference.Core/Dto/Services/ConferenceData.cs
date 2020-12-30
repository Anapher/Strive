using System.Collections.Generic;
using System.Text.Json;
using PaderConference.Core.Domain.Entities;

namespace PaderConference.Core.Dto.Services
{
    /// <summary>
    ///     The object contains the mutable data the conference has and that may be changed by the user
    /// </summary>
    public class ConferenceData
    {
        public ConferenceConfiguration Configuration { get; init; } = new();

        public Dictionary<PermissionType, Dictionary<string, JsonElement>> Permissions { get; init; } = new();

        public static ConferenceData FromConference(Conference conference)
        {
            return new() {Configuration = conference.Configuration, Permissions = conference.Permissions};
        }
    }
}
