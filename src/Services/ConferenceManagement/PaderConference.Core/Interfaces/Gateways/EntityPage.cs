using System.Collections.Generic;

namespace PaderConference.Core.Interfaces.Gateways
{
    public class EntityPage<T>
    {
        public EntityPage(IReadOnlyList<T> result, int totalLength)
        {
            Result = result;
            TotalLength = totalLength;
        }

        public IReadOnlyList<T> Result { get; }

        public int TotalLength { get; }
    }
}
