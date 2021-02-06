using System.Threading.Tasks;

namespace PaderConference.Core.Services
{
    public interface IConferenceServiceManager<TService> : IConferenceServiceManager where TService : IConferenceService
    {
        new ValueTask<TService> GetService(string conferenceId);
    }
}
