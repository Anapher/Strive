using System.Threading.Tasks;
using PaderConference.Core.Domain.Entities;

namespace PaderConference.Core.Services
{
    public interface IServiceMessage
    {
        Participant Participant { get; }

        string ConnectionId { get; }

        Task SendToCallerAsync(string method, object dto);
    }
}