using System.Threading.Tasks;

namespace PaderConference.Infrastructure.Services
{
    public interface IConferenceServiceManager<TService> : IConferenceServiceManager where TService : IConferenceService
    {
        new ValueTask<TService> GetService(string conferenceId);
    }
}
