using System;
using Strive.Core;
using Strive.Core.Dto;
using Strive.Core.Services;

namespace Strive.Extensions
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
