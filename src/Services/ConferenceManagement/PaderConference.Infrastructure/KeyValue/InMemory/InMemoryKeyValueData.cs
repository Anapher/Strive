using System.Collections.Generic;
using Nito.AsyncEx;

namespace PaderConference.Infrastructure.KeyValue.InMemory
{
    public class InMemoryKeyValueData
    {
        public readonly AsyncReaderWriterLock Lock = new();
        public readonly Dictionary<string, object> Data = new();
    }
}
