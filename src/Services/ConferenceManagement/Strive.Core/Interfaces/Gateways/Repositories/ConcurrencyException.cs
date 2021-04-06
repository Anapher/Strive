using System;

namespace Strive.Core.Interfaces.Gateways.Repositories
{
    public class ConcurrencyException : Exception
    {
        public ConcurrencyException(string message) : base(message)
        {
        }
    }
}
