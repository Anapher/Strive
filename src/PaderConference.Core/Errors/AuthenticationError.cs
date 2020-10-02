namespace PaderConference.Core.Errors
{
    public class AuthenticationError : DomainError
    {
        public AuthenticationError(string message, ErrorCode code) : base(ErrorType.Authentication, message, code)
        {
        }

        public static AuthenticationError UserNotFound => new AuthenticationError("The user was not found.", ErrorCode.UserNotFound);
        public static AuthenticationError InvalidToken => new AuthenticationError("Invalid token.", ErrorCode.InvalidToken);

    }
}
