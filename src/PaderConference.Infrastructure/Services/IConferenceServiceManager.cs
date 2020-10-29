using System;
using System.Threading.Tasks;

namespace PaderConference.Infrastructure.Services
{
    public interface IConferenceServiceManager
    {
        Type ServiceType { get; }

        ValueTask Close(string conferenceId);

        ValueTask<IConferenceService> GetService(string conferenceId);
    }
}