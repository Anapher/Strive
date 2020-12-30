#pragma warning disable 8618
// this file is validated so the nullable annotations are enforced

using System.Collections.Generic;
using System.Text.Json;
using PaderConference.Core.Domain.Entities;

namespace PaderConference.Models.Request
{
    public class CreateConferenceRequestDto
    {
        public ConferenceConfiguration Configuration { get; set; }

        public Dictionary<PermissionType, Dictionary<string, JsonElement>>? Permissions { get; set; }
    }
}
