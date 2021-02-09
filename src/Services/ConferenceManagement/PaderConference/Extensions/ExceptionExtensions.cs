using System;
using PaderConference.Core;
using PaderConference.Core.Dto;
using PaderConference.Core.Services;

namespace PaderConference.Extensions
{
    public static class ExceptionExtensions
    {
        public static Error ToError(this Exception e)
        {
            if (e is IdErrorException idError)
                return idError.Error;

            return ConferenceError.UnexpectedError("An unexpected error occurred");
        }
    }
}
