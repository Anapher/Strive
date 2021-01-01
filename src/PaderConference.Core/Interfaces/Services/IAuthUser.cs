using System.Threading.Tasks;

namespace PaderConference.Core.Interfaces.Services
{
    public interface IAuthUser
    {
        string Id { get; }

        string Name { get; }

        ValueTask<bool> ValidatePassword(string password);
    }
}
