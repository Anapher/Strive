using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using PaderConference.Core.Interfaces.Services;

namespace PaderConference.Infrastructure.Auth.AuthService
{
    public class OptionsAuthService : IAuthService
    {
        private readonly UserCredentialsOptions _options;

        public OptionsAuthService(IOptions<UserCredentialsOptions> options)
        {
            _options = options.Value;
        }

        public ValueTask<IAuthUser?> FindUser(string username)
        {
            if (_options.Users != null && _options.Users.TryGetValue(username, out var data))
                return new ValueTask<IAuthUser?>(new OptionsAuthUser(data));

            return new ValueTask<IAuthUser?>((IAuthUser?) null);
        }
    }
}
