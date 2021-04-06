using System;
using Strive.Core.Dto;

namespace Strive.Core.Services
{
    public class IdErrorException : Exception
    {
        public Error Error { get; }

        public IdErrorException(Error error)
        {
            Error = error;
        }

        public override string ToString()
        {
            return Error.ToString();
        }
    }
}
