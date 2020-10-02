using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System;

namespace PaderConference.Infrastructure.Identity
{
    public class BCryptPasswordHasher<TUser> : PasswordHasher<TUser> where TUser : class
    {
        /// <summary>
        ///  Returns a hashed representation of the supplied password for the specified user.
        /// </summary>
        public override string HashPassword(TUser user, string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        /// <summary>
        /// Returns a Microsoft.AspNetCore.Identity.PasswordVerificationResult indicating the result of a password hash comparison.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="hashedPassword">The hash value for a user's stored password.</param>
        /// <param name="providedPassword"> The password supplied for comparison.</param>
        public override PasswordVerificationResult VerifyHashedPassword(TUser user, string hashedPassword, string providedPassword)
        {
            if (hashedPassword == null) { throw new ArgumentNullException(nameof(hashedPassword)); }
            if (providedPassword == null) { throw new ArgumentNullException(nameof(providedPassword)); }

            if (BCrypt.Net.BCrypt.Verify(providedPassword, hashedPassword))
            {
                return PasswordVerificationResult.Success;
            }
            else
            {
                return PasswordVerificationResult.Failed;
            }
        }
    }

}
