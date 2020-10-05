#pragma warning disable CS8618 // Non-nullable field is uninitialized. Validators guarantee that.

namespace PaderConference.Models.Request
{
    public class LoginRequestDto
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}