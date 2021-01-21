using System.Threading.Tasks;
using PaderConference.Core.Interfaces.Services;
using PaderConference.Core.Signaling;

namespace PaderConference.Core.Services.Chat.Filters
{
    public interface IMessageFilter
    {
        bool ShowMessageTo(string participantId);

        Task SendAsync(ISignalMessenger clients, IConnectionMapping connectionMapping, string conferenceId,
            string method, object dto);
    }
}