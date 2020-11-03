using System.Threading.Tasks;
using PaderConference.Core.Interfaces.Services;
using PaderConference.Core.Signaling;

namespace PaderConference.Core.Services.Chat.Filters
{
    /// <summary>
    ///     Send the message to all participants
    /// </summary>
    public class AtAllFilter : IMessageFilter
    {
        public bool ShowMessageTo(string connectionId)
        {
            return true;
        }

        public Task SendAsync(ISignalMessenger clients, IConnectionMapping connectionMapping, string conferenceId,
            string method, object dto)
        {
            return clients.SendToConferenceAsync(conferenceId, method, dto);
        }
    }
}
