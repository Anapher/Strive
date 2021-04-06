using Strive.Core.Dto;

namespace Strive.Core.Errors
{
    public class AuthenticationError : ErrorsProvider<ErrorCode>
    {
        public static Error UserNotFound => BadRequest("The user was not found.", ErrorCode.UserNotFound);
        public static Error InvalidPassword => BadRequest("The password is invalid.", ErrorCode.InvalidPassword);
        public static Error InvalidToken => Conflict("Invalid token.", ErrorCode.InvalidToken);
        public static Error TokenExpired => Conflict("Your token expired.", ErrorCode.TokenExpired);
    }
}
