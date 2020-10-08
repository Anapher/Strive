using System.Collections.Generic;
using PaderConference.Infrastructure.Serialization;

namespace PaderConference.Infrastructure.Services.Synchronization
{
    public class SynchronizedObjectUpdatedDto
    {
        public SynchronizedObjectUpdatedDto(string name, List<SerializableJsonPatchOperation> patch)
        {
            Name = name;
            Patch = patch;
        }

        public string Name { get; }

        public List<SerializableJsonPatchOperation> Patch { get; }
    }
}