using PaderConference.Core.Dto;
using PaderConference.Core.Errors;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PaderConference.Infrastructure.Identity
{
    internal static class IdentityErrorMapper
    {
        public static Error MapToError(IEnumerable<IdentityError> errors)
        {
            var firstError = errors.First();
            var code = Enum.Parse<ErrorCode>($"Identity_{firstError.Code}");

            if (code >= ErrorCode.Identity_PasswordTooShort && code <= ErrorCode.Identity_PasswordRequiresUpper)
                return new FieldValidationError("Password", firstError.Description, code);

            if (code == ErrorCode.Identity_InvalidUserName || code == ErrorCode.Identity_DuplicateUserName)
                return new FieldValidationError("UserName", firstError.Description, code);

            if (code == ErrorCode.Identity_InvalidEmail || code == ErrorCode.Identity_DuplicateEmail)
                return new FieldValidationError("Email", firstError.Description, code);

            return new AuthenticationError(firstError.Description, code);
        }
    }
}
