using Microsoft.Extensions.Options;
using Strive.Infrastructure.KeyValue;
using Strive.Infrastructure.KeyValue.InMemory;
using Strive.Infrastructure.Tests.KeyValue.Scripts.Base;

namespace Strive.Infrastructure.Tests.KeyValue.Scripts
{
    public class
        JoinedParticipantsRepository_RemoveParticipant_InMemoryTests :
            JoinedParticipantsRepository_RemoveParticipant_Tests
    {
        public JoinedParticipantsRepository_RemoveParticipant_InMemoryTests() : base(
            new InMemoryKeyValueDatabase(new InMemoryKeyValueData(),
                new OptionsWrapper<KeyValueDatabaseOptions>(new KeyValueDatabaseOptions())))
        {
        }
    }
}
