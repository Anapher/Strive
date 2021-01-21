using System.Threading.Tasks;

namespace PaderConference.Core.Signaling
{
    public interface ISignalMessenger
    {
        Task SendToConferenceAsync(string conferenceId, string method, object? arg);

        Task SendToConnectionAsync(string connectionId, string method, object? arg);
    }
}
