#pragma warning disable 8618
using System.Collections.Generic;

namespace PaderConference.Models.Signal
{
    public class ParticipantDto
    {
        public string ParticipantId { get; set; }

        public string Role { get; set; }

        public string? DisplayName { get; set; }

        public IReadOnlyDictionary<string, string> Attributes { get; set; }
    }
}