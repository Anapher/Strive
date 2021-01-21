using System.Threading.Tasks;
using PaderConference.Core.Interfaces.Services;

namespace PaderConference.Infrastructure.Auth.AuthService
{
    public class OptionsAuthUser : IAuthUser
    {
        private readonly OptionsUserData _data;

        public OptionsAuthUser(OptionsUserData data)
        {
            _data = data;
        }

        public string Id => _data.Id;

        public string Name => _data.DisplayName;

        public ValueTask<bool> ValidatePassword(string password)
        {
            return new(_data.Password == password);
        }
    }
}
