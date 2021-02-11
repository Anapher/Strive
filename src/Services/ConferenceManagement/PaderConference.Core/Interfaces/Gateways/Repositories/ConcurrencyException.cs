using System;

namespace PaderConference.Core.Interfaces.Gateways.Repositories
{
    public class ConcurrencyException : Exception
    {
        public ConcurrencyException(string message) : base(message)
        {
        }
    }
}
