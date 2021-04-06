using System.Collections.Generic;

namespace Strive.Core.Interfaces.Gateways
{
    public record EntityPage<T>(IReadOnlyList<T> Result, int TotalLength);
}
