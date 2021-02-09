using System;
using PaderConference.Core.Dto;

namespace PaderConference.Core.Services
{
    public class IdErrorException : Exception
    {
        public Error Error { get; }

        public IdErrorException(Error error)
        {
            Error = error;
        }
    }
}
