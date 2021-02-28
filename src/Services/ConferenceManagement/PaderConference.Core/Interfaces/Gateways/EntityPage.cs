using System.Collections.Generic;

namespace PaderConference.Core.Interfaces.Gateways
{
    public record EntityPage<T>(IReadOnlyList<T> Result, int TotalLength);
}
