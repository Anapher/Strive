using System.Threading.Tasks;
using PaderConference.Core.Signaling;

namespace PaderConference.Core.Services.Chat.Filters
{
    public interface IMessageFilter
    {
        bool ShowMessageTo(string connectionId);

        Task SendAsync(ISignalMessenger clients, string conferenceId, string method, object dto);
    }
}