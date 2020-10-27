using System.Threading.Tasks;

namespace PaderConference.Infrastructure.Services
{
    public interface IConferenceServiceManager
    {
        ValueTask Close(string conferenceId);

        ValueTask<IConferenceService> GetService(string conferenceId);
    }
}